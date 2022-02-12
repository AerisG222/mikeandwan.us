using Dapper;
using Maw.Data.Abstractions;
using Maw.Domain.Models;
using Maw.Domain.Models.Photos;

namespace Maw.Data;

public class PhotoRepository
    : Repository, IPhotoRepository
{
    public PhotoRepository(string connectionString)
        : base(connectionString)
    {

    }

    public Task<IEnumerable<short>> GetYearsAsync(string[] roles)
    {
        return RunAsync(conn =>
            conn.QueryAsync<short>(
                "SELECT * FROM photo.get_years(@roles);",
                new
                {
                    roles
                }
            )
        );
    }

    public Task<IEnumerable<Category>> GetAllCategoriesAsync(string[] roles)
    {
        return InternalGetCategoriesAsync(roles);
    }

    public Task<IEnumerable<Category>> GetCategoriesForYearAsync(short year, string[] roles)
    {
        return InternalGetCategoriesAsync(roles, year);
    }

    public Task<IEnumerable<Category>> GetRecentCategoriesAsync(short sinceId, string[] roles)
    {
        return InternalGetCategoriesAsync(roles, sinceCategoryId: sinceId);
    }

    public async Task<Category?> GetCategoryAsync(short categoryId, string[]? roles)
    {
        var result = await InternalGetCategoriesAsync(roles, categoryId: categoryId);

        return result.FirstOrDefault();
    }

    public Task<IEnumerable<Photo>> GetPhotosForCategoryAsync(short categoryId, string[] roles)
    {
        return InternalGetPhotosAsync(roles, categoryId);
    }

    public async Task<Photo?> GetPhotoAsync(int photoId, string[] roles)
    {
        var result = await InternalGetPhotosAsync(roles, photoId: photoId);

        return result.FirstOrDefault();
    }

    public async Task<Photo?> GetRandomAsync(string[] roles)
    {
        var results = await GetRandomAsync(1, roles);

        return results.First();
    }

    public Task<IEnumerable<Photo>> GetRandomAsync(byte count, string[] roles)
    {
        return RunAsync(async conn =>
        {
            var rows = await conn.QueryAsync(
                "SELECT * FROM photo.get_random_photos(@roles, @count)",
                new
                {
                    roles,
                    count
                }
            );

            return rows.Select(BuildPhoto);
        });
    }

    public Task<Detail?> GetDetailAsync(int photoId, string[] roles)
    {
        return RunAsync(conn =>
            conn.QuerySingleOrDefaultAsync<Detail?>(
                "SELECT * FROM photo.get_photo_metadata(@roles, @photoId);",
                new
                {
                    roles,
                    photoId
                }
            )
        );
    }

    public Task<IEnumerable<Comment>> GetCommentsAsync(int photoId, string[] roles)
    {
        return RunAsync(conn =>
            conn.QueryAsync<Comment>(
                "SELECT * FROM photo.get_comments(@photoId, @roles);",
                new
                {
                    photoId,
                    roles
                }
            )
        );
    }

    public Task<Rating?> GetRatingsAsync(int photoId, string username, string[] roles)
    {
        return RunAsync(conn =>
            conn.QuerySingleOrDefaultAsync<Rating?>(
                "SELECT * FROM photo.get_ratings(@photoId, @username, @roles);",
                new
                {
                    photoId,
                    username = username?.ToLowerInvariant(),
                    roles
                }
            )
        );
    }

    public Task<GpsDetail?> GetGpsDetailAsync(int photoId, string[] roles)
    {
        return RunAsync(async conn =>
        {
            var result = await conn.QuerySingleOrDefaultAsync<GpsSourceOverride>(
                "SELECT * FROM photo.get_gps(@photoId, @roles);",
                new
                {
                    photoId,
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

    public Task InsertCommentAsync(int photoId, string username, string comment, string[] roles)
    {
        return RunAsync(async conn =>
        {
            var result = await conn.QuerySingleOrDefaultAsync<int>(
                "SELECT * FROM photo.save_comment(@username, @photoId, @message, @entryDate, @roles);",
                new
                {
                    username = username.ToLowerInvariant(),
                    photoId,
                    message = comment,
                    entryDate = DateTime.Now,
                    roles
                }
            );

            if (result <= 0)
            {
                throw new Exception("Did not save photo comment!");
            }
        });
    }

    public Task<float?> SaveRatingAsync(int photoId, string username, short rating, string[] roles)
    {
        return RunAsync(async conn =>
        {
            var result = await conn.QueryAsync<long>(
                "SELECT * FROM photo.save_rating(@photoId, @username, @score, @roles);",
                new
                {
                    photoId,
                    username = username.ToLowerInvariant(),
                    score = rating,
                    roles
                }
            );

            return (await GetRatingsAsync(photoId, username, roles))?.AverageRating;
        });
    }

    public Task<float?> RemoveRatingAsync(int photoId, string username, string[] roles)
    {
        return RunAsync(async conn =>
        {
            var result = await conn.QueryAsync<long>(
                "SELECT * FROM photo.save_rating(@photoId, @username, @score, @roles);",
                new
                {
                    photoId,
                    username = username.ToLowerInvariant(),
                    score = 0,
                    roles
                }
            );

            return (await GetRatingsAsync(photoId, username, roles))?.AverageRating;
        });
    }

    public Task SetGpsOverrideAsync(int photoId, GpsCoordinate gps, string username)
    {
        return RunAsync(conn =>
            conn.QueryAsync<long>(
                "SELECT * FROM photo.set_gps_override(@photoId, @latitude, @longitude, @username, @updateDate);",
                new
                {
                    photoId,
                    latitude = gps.Latitude,
                    longitude = gps.Longitude,
                    username = username.ToLowerInvariant(),
                    updateDate = DateTime.Now
                }
            )
        );
    }

    public Task<long> SetCategoryTeaserAsync(short categoryId, int photoId)
    {
        return RunAsync(conn =>
            conn.QueryFirstAsync<long>(
                @"SELECT * FROM photo.set_category_teaser(@categoryId, @photoId);",
                new
                {
                    categoryId,
                    photoId
                }
            )
        );
    }

    public Task<IEnumerable<CategoryAndRoles>> GetCategoriesAndRolesAsync()
    {
        return RunAsync(conn => conn.QueryAsync<CategoryAndRoles>("SELECT * FROM photo.get_category_roles();"));
    }

    Task<IEnumerable<Category>> InternalGetCategoriesAsync(string[]? roles, short? year = null, short? categoryId = null, short? sinceCategoryId = null)
    {
        return RunAsync(async conn =>
        {
            var rows = await conn.QueryAsync(
                "SELECT * FROM photo.get_categories(@roles, @year, @categoryId, @sinceCategoryId);",
                new
                {
                    roles,
                    year,
                    categoryId,
                    sinceCategoryId
                }
            );

            return rows.Select(BuildCategory);
        });
    }

    Task<IEnumerable<Photo>> InternalGetPhotosAsync(string[] roles, short? categoryId = null, int? photoId = null)
    {
        return RunAsync(async conn =>
        {
            var rows = await conn.QueryAsync(
                "SELECT * FROM photo.get_photos(@roles, @categoryId, @photoId);",
                new
                {
                    roles,
                    categoryId,
                    photoId
                }
            );

            return rows.Select(BuildPhoto);
        });
    }

    Category BuildCategory(dynamic row)
    {
        var category = new Category();

        category.Id = (short)row.id;
        category.Year = (short)row.year;
        category.Name = (string)row.name;
        category.CreateDate = GetValueOrDefault<DateTime>(row.create_date);
        category.Latitude = row.latitude;
        category.Longitude = row.longitude;
        category.PhotoCount = GetValueOrDefault<int>(row.photo_count);
        category.TotalSizeXs = GetValueOrDefault<long>(row.total_size_xs);
        category.TotalSizeXsSq = GetValueOrDefault<long>(row.total_size_xs_sq);
        category.TotalSizeSm = GetValueOrDefault<long>(row.total_size_sm);
        category.TotalSizeMd = GetValueOrDefault<long>(row.total_size_md);
        category.TotalSizeLg = GetValueOrDefault<long>(row.total_size_lg);
        category.TotalSizePrt = GetValueOrDefault<long>(row.total_size_prt);
        category.TotalSizeSrc = GetValueOrDefault<long>(row.total_size_src);
        category.TotalSize = GetValueOrDefault<long>(row.total_size);
        category.IsMissingGpsData = GetValueOrDefault<bool>(row.is_missing_gps_data);

        category.TeaserImage = BuildMultimediaInfo(row.teaser_photo_path, row.teaser_photo_width, row.teaser_photo_height, row.teaser_photo_size);
        category.TeaserImageSq = BuildMultimediaInfo(row.teaser_photo_sq_path, row.teaser_photo_sq_width, row.teaser_photo_sq_height, row.teaser_photo_sq_size);

        return category;
    }

    Photo BuildPhoto(dynamic row)
    {
        var photo = new Photo();

        photo.Id = (int)row.id;
        photo.CategoryId = (short)row.category_id;
        photo.CreateDate = GetValueOrDefault<DateTime>(row.create_date);
        photo.Latitude = row.latitude;
        photo.Longitude = row.longitude;

        photo.XsInfo = BuildMultimediaInfo(row.xs_path, row.xs_width, row.xs_height, row.xs_size);
        photo.XsSqInfo = BuildMultimediaInfo(row.xs_sq_path, row.xs_sq_width, row.xs_sq_height, row.xs_sq_size);
        photo.SmInfo = BuildMultimediaInfo(row.sm_path, row.sm_width, row.sm_height, row.sm_size);
        photo.MdInfo = BuildMultimediaInfo(row.md_path, row.md_width, row.md_height, row.md_size);
        photo.LgInfo = BuildMultimediaInfo(row.lg_path, row.lg_width, row.lg_height, row.lg_size);
        photo.PrtInfo = BuildMultimediaInfo(row.prt_path, row.prt_width, row.prt_height, row.prt_size);
        photo.SrcInfo = BuildMultimediaInfo(row.src_path, row.src_width, row.src_height, row.src_size);

        return photo;
    }
}
