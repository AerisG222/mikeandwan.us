using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Maw.Domain;
using Maw.Domain.Videos;

namespace Maw.Data;

public class VideoRepository
    : Repository, IVideoRepository
{
    public VideoRepository(string connectionString)
        : base(connectionString)
    {

    }

    public Task<IEnumerable<short>> GetYearsAsync(string[] roles)
    {
        return RunAsync(conn =>
            conn.QueryAsync<short>(
                "SELECT * FROM video.get_years(@roles);",
                new { roles }
            )
        );
    }

    public Task<IEnumerable<Category>> GetAllCategoriesAsync(string[] roles)
    {
        return InternalGetCategoriesAsync(roles);
    }

    public Task<IEnumerable<Category>> GetCategoriesAsync(short year, string[] roles)
    {
        return InternalGetCategoriesAsync(roles, year);
    }

    public async Task<Category?> GetCategoryAsync(short categoryId, string[] roles)
    {
        var result = await InternalGetCategoriesAsync(roles, categoryId: categoryId);

        return result.FirstOrDefault();
    }

    public Task<IEnumerable<Video>> GetVideosInCategoryAsync(short categoryId, string[] roles)
    {
        return InternalGetVideosAsync(roles, categoryId);
    }

    public async Task<Video?> GetVideoAsync(short id, string[] roles)
    {
        var result = await InternalGetVideosAsync(roles, videoId: id);

        return result.FirstOrDefault();
    }

    public Task<IEnumerable<Comment>> GetCommentsAsync(short videoId, string[] roles)
    {
        return RunAsync(conn =>
            conn.QueryAsync<Comment>(
                "SELECT * FROM video.get_comments(@videoId, @roles);",
                new
                {
                    videoId,
                    roles
                }
            )
        );
    }

    public Task<GpsDetail?> GetGpsDetailAsync(int videoId, string[] roles)
    {
        return RunAsync(async conn =>
        {
            var result = await conn.QuerySingleOrDefaultAsync<GpsSourceOverride>(
                "SELECT * FROM video.get_gps(@videoId, @roles);",
                new
                {
                    videoId,
                    roles
                }
            );

            if (result == null)
            {
                return null;
            }

            var detail = new GpsDetail();

            if (result.SourceLatitude != null && result.SourceLongitude != null)
            {
                detail.Source = new GpsCoordinate()
                {
                    Latitude = (float)result.SourceLatitude,
                    Longitude = (float)result.SourceLongitude
                };
            }

            if (result.OverrideLatitude != null && result.OverrideLongitude != null)
            {
                detail.Override = new GpsCoordinate()
                {
                    Latitude = (float)result.OverrideLatitude,
                    Longitude = (float)result.OverrideLongitude
                };
            }

            return detail;
        });
    }

    public Task<Rating?> GetRatingsAsync(short videoId, string username, string[] roles)
    {
        return RunAsync(conn =>
            conn.QuerySingleOrDefaultAsync<Rating?>(
                "SELECT * FROM video.get_ratings(@videoId, @username, @roles);",
                new
                {
                    videoId,
                    username = username?.ToLowerInvariant(),
                    roles
                }
            )
        );
    }

    public Task InsertCommentAsync(short videoId, string username, string comment, string[] roles)
    {
        return RunAsync(async conn =>
        {
            var result = await conn.QuerySingleOrDefaultAsync<int>(
                "SELECT * FROM video.save_comment(@username, @videoId, @message, @entryDate, @roles);",
                new
                {
                    username = username.ToLowerInvariant(),
                    videoId,
                    message = comment,
                    entryDate = DateTime.Now,
                    roles
                }
            );

            if (result <= 0)
            {
                throw new Exception("Did not save video comment!");
            }
        });
    }

    public Task<float?> SaveRatingAsync(short videoId, string username, short rating, string[] roles)
    {
        return RunAsync(async conn =>
        {
            var result = await conn.QueryAsync<long>(
                "SELECT * FROM video.save_rating(@videoId, @username, @score, @roles);",
                new
                {
                    videoId,
                    username = username.ToLowerInvariant(),
                    score = rating,
                    roles
                }
            );

            return (await GetRatingsAsync(videoId, username, roles))?.AverageRating;
        });
    }

    public Task<float?> RemoveRatingAsync(short videoId, string username, string[] roles)
    {
        return RunAsync(async conn =>
        {
            var result = await conn.QueryAsync<long>(
                @"SELECT * FROM video.save_rating(@videoId, @username, @score, @roles);",
                new
                {
                    videoId,
                    username = username.ToLowerInvariant(),
                    score = 0,
                    roles
                }
            );

            return (await GetRatingsAsync(videoId, username, roles))?.AverageRating;
        });
    }

    public Task SetGpsOverrideAsync(int videoId, GpsCoordinate gps, string username)
    {
        return RunAsync(conn =>
            conn.QueryAsync<long>(
                "SELECT * FROM video.set_gps_override(@videoId, @latitude, @longitude, @username, @updateDate);",
                new
                {
                    videoId,
                    latitude = gps.Latitude,
                    longitude = gps.Longitude,
                    username = username.ToLowerInvariant(),
                    updateDate = DateTime.Now
                }
            )
        );
    }

    public Task<long> SetCategoryTeaserAsync(short categoryId, int videoId)
    {
        return RunAsync(conn =>
            conn.QueryFirstAsync<long>(
                @"SELECT * FROM video.set_category_teaser(@categoryId, @videoId);",
                new
                {
                    categoryId,
                    videoId
                }
            )
        );
    }

    Task<IEnumerable<Category>> InternalGetCategoriesAsync(string[] roles, short? year = null, short? categoryId = null)
    {
        return RunAsync(async conn =>
        {
            var rows = await conn.QueryAsync(
                "SELECT * FROM video.get_categories(@roles, @year, @categoryId);",
                new
                {
                    roles,
                    year,
                    categoryId
                }
            );

            return rows.Select(BuildCategory);
        });
    }

    Task<IEnumerable<Video>> InternalGetVideosAsync(string[] roles, short? categoryId = null, short? videoId = null)
    {
        return RunAsync(async conn =>
        {
            var rows = await conn.QueryAsync(
                "SELECT * FROM video.get_videos(@roles, @categoryId, @videoId);",
                new
                {
                    roles,
                    categoryId,
                    videoId
                }
            );

            return rows.Select(BuildVideo);
        });
    }

    Category BuildCategory(dynamic row)
    {
        var category = new Category();

        category.Id = (short)row.id;
        category.Name = (string)row.name;
        category.Year = (short)row.year;
        category.CreateDate = GetValueOrDefault<DateTime>(row.create_date);
        category.Latitude = row.latitude;
        category.Longitude = row.longitude;
        category.VideoCount = GetValueOrDefault<int>(row.video_count);
        category.TotalDuration = GetValueOrDefault<int>(row.total_duration);
        category.TotalSizeThumbnail = GetValueOrDefault<long>(row.total_size_thumb);
        category.TotalSizeThumbnailSq = GetValueOrDefault<long>(row.total_size_thumb_sq);
        category.TotalSizeScaled = GetValueOrDefault<long>(row.total_size_scaled);
        category.TotalSizeFull = GetValueOrDefault<long>(row.total_size_full);
        category.TotalSizeRaw = GetValueOrDefault<long>(row.total_size_raw);
        category.TotalSize = GetValueOrDefault<long>(row.total_size);
        category.IsMissingGpsData = GetValueOrDefault<bool>(row.is_missing_gps_data);

        category.TeaserImage = BuildMultimediaInfo(row.teaser_image_path, row.teaser_image_width, row.teaser_image_height, row.teaser_image_size);
        category.TeaserImageSq = BuildMultimediaInfo(row.teaser_image_sq_path, row.teaser_image_sq_width, row.teaser_image_sq_height, row.teaser_image_sq_size);

        return category;
    }

    Video BuildVideo(dynamic row)
    {
        var video = new Video();

        video.Id = (int)row.id;
        video.CategoryId = (short)row.category_id;
        video.CreateDate = GetValueOrDefault<DateTime>(row.create_date);
        video.Latitude = row.latitude;
        video.Longitude = row.longitude;
        video.Duration = GetValueOrDefault<short>(row.duration);

        video.Thumbnail = BuildMultimediaInfo(row.thumb_path, row.thumb_width, row.thumb_height, row.thumb_size);
        video.ThumbnailSq = BuildMultimediaInfo(row.thumb_sq_path, row.thumb_sq_width, row.thumb_sq_height, row.thumb_sq_size);
        video.VideoScaled = BuildMultimediaInfo(row.scaled_path, row.scaled_width, row.scaled_height, row.scaled_size);
        video.VideoFull = BuildMultimediaInfo(row.full_path, row.full_width, row.full_height, row.full_size);
        video.VideoRaw = BuildMultimediaInfo(row.raw_path, row.raw_width, row.raw_height, row.raw_size);

        return video;
    }
}
