using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Maw.Domain;
using Maw.Domain.Photos;


namespace Maw.Data
{
	public class PhotoRepository
        : Repository, IPhotoRepository
	{
        public PhotoRepository(string connectionString)
            : base(connectionString)
        {

        }


		public Task<IEnumerable<short>> GetYearsAsync()
        {
            return RunAsync(conn =>
                conn.QueryAsync<short>(
                    "SELECT * FROM photo.get_years();"
                )
            );
        }


        public Task<IEnumerable<Category>> GetAllCategoriesAsync(bool allowPrivate)
        {
            return InternalGetCategoriesAsync(allowPrivate);
        }


        public Task<IEnumerable<Category>> GetCategoriesForYearAsync(short year, bool allowPrivate)
		{
            return InternalGetCategoriesAsync(allowPrivate, year);
		}


        public Task<IEnumerable<Category>> GetRecentCategoriesAsync(short sinceId, bool allowPrivate)
        {
            return InternalGetCategoriesAsync(allowPrivate, sinceCategoryId: sinceId);
        }


        public async Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate)
        {
            var result = await InternalGetCategoriesAsync(allowPrivate, categoryId: categoryId).ConfigureAwait(false);

            return result.FirstOrDefault();
        }


		public Task<IEnumerable<Photo>> GetPhotosForCategoryAsync(short categoryId, bool allowPrivate)
		{
            return InternalGetPhotosAsync(allowPrivate, categoryId);
		}


		public async Task<Photo> GetPhotoAsync(int photoId, bool allowPrivate)
		{
            var result = await InternalGetPhotosAsync(allowPrivate, photoId: photoId).ConfigureAwait(false);

            return result.FirstOrDefault();
		}


        public async Task<Photo> GetRandomAsync(bool allowPrivate)
        {
            var results = await GetRandomAsync(1, allowPrivate).ConfigureAwait(false);

            return results.First();
        }


        public Task<IEnumerable<Photo>> GetRandomAsync(byte count, bool allowPrivate)
        {
            return RunAsync(async conn => {
                var rows = await conn.QueryAsync(
                    "SELECT * FROM photo.get_random_photos(@allowPrivate, @count)",
                    new {
                        allowPrivate,
                        count
                    }
                ).ConfigureAwait(false);

                return rows.Select(BuildPhoto);
            });
        }


		public Task<Detail> GetDetailAsync(int photoId, bool allowPrivate)
		{
            return RunAsync(conn =>
                conn.QuerySingleOrDefaultAsync<Detail>(
                    "SELECT * FROM photo.get_photo_metadata(@allowPrivate, @photoId);",
                    new {
                        allowPrivate,
                        photoId
                    }
                )
            );
		}


		public Task<IEnumerable<Comment>> GetCommentsAsync(int photoId)
		{
            return RunAsync(conn =>
                conn.QueryAsync<Comment>(
                    "SELECT * FROM photo.get_comments(@photoId);",
                    new { photoId = photoId }
                )
            );
		}


		public Task<Rating> GetRatingsAsync(int photoId, string username)
		{
            return RunAsync(conn =>
                conn.QuerySingleOrDefaultAsync<Rating>(
                    "SELECT * FROM photo.get_ratings(@photoId, @username);",
                    new {
                        photoId = photoId,
                        username = username?.ToLowerInvariant()
                    }
                )
            );
		}


        public Task<GpsDetail> GetGpsDetailAsync(int photoId)
		{
            return RunAsync(async conn => {
                var result = await conn.QuerySingleOrDefaultAsync<dynamic>(
                    "SELECT * FROM photo.get_gps(@photoId);",
                    new { photoId }
                ).ConfigureAwait(false);

                if(result == null)
                {
                    return null;
                }

                var detail = new GpsDetail();

                if(result.source_latitude != DBNull.Value && result.source_longitude != DBNull.Value)
                {
                    detail.Source = new GpsCoordinate()
                    {
                        Latitude = result.source_latitude,
                        Longitude = result.source_longitude
                    };
                }

                if(result.override_latitude != DBNull.Value && result.override_longitude != DBNull.Value)
                {
                    detail.Override = new GpsCoordinate()
                    {
                        Latitude = result.override_latitude,
                        Longitude = result.override_longitude
                    };
                }

                return detail;
            });
		}


		public Task<int> InsertCommentAsync(int photoId, string username, string comment)
        {
            return RunAsync(async conn => {
                var result = await conn.QuerySingleOrDefaultAsync<int>(
                    "SELECT * FROM photo.save_comment(@username, @photoId, @message, @entryDate);",
                    new {
                        username = username.ToLowerInvariant(),
                        photoId = photoId,
                        message = comment,
                        entryDate = DateTime.Now
                    }
                ).ConfigureAwait(false);

                if(result <= 0)
				{
					throw new Exception("Did not save photo comment!");
				}

                return result;
            });
        }


		public Task<float?> SaveRatingAsync(int photoId, string username, short rating)
        {
            return RunAsync(async conn => {
                var result = await conn.QueryAsync<long>(
                    "SELECT * FROM photo.save_rating(@photoId, @username, @score);",
                    new {
                        photoId = photoId,
                        username = username.ToLowerInvariant(),
                        score = rating
                    }
                ).ConfigureAwait(false);

                return (await GetRatingsAsync(photoId, username).ConfigureAwait(false))?.AverageRating;
            });
        }


		public Task<float?> RemoveRatingAsync(int photoId, string username)
		{
            return RunAsync(async conn => {
                var result = await conn.QueryAsync<long>(
                    "SELECT * FROM photo.save_rating(@photoId, @username, @score);",
                    new {
                        photoId = photoId,
                        username = username.ToLowerInvariant(),
                        score = 0
                    }
                ).ConfigureAwait(false);

                return (await GetRatingsAsync(photoId, username).ConfigureAwait(false))?.AverageRating;
            });
		}


        public Task SetGpsOverrideAsync(int photoId, GpsCoordinate gps, string username)
		{
            return RunAsync(conn =>
                conn.QueryAsync<long>(
                    "SELECT * FROM photo.set_gps_override(@photoId, @latitude, @longitude, @username, @updateDate);",
                    new {
                        photoId,
                        latitude = gps.Latitude,
                        longitude = gps.Longitude,
                        username = username.ToLowerInvariant(),
                        updateDate = DateTime.Now
                    }
                )
            );
		}


        Task<IEnumerable<Category>> InternalGetCategoriesAsync(bool allowPrivate, short? year = null, short? categoryId = null, short? sinceCategoryId = null)
        {
            return RunAsync(async conn => {
                var rows = await conn.QueryAsync(
                    "SELECT * FROM photo.get_categories(@allowPrivate, @year, @categoryId, @sinceCategoryId);",
                    new {
                        allowPrivate,
                        year,
                        categoryId,
                        sinceCategoryId
                    }
                ).ConfigureAwait(false);

                return rows.Select(BuildCategory);
            });
        }


        Task<IEnumerable<Photo>> InternalGetPhotosAsync(bool allowPrivate, short? categoryId = null, int? photoId = null)
		{
            return RunAsync(async conn => {
                var rows = await conn.QueryAsync(
                    "SELECT * FROM photo.get_photos(@allowPrivate, @categoryId, @photoId);",
                    new {
                        allowPrivate,
                        categoryId,
                        photoId
                    }
                ).ConfigureAwait(false);

                return rows.Select(BuildPhoto);
            });
		}


        Category BuildCategory(dynamic row)
        {
            var category = new Category();

            category.Id = (short) row.id;
            category.Year = (short) row.year;
            category.Name = (string) row.name;
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

            category.TeaserImage = BuildMultimediaInfo(row.teaser_photo_path, row.teaser_photo_width, row.teaser_photo_height, row.teaser_photo_size);
            category.TeaserImageSq = BuildMultimediaInfo(row.teaser_photo_sq_path, row.teaser_photo_sq_width, row.teaser_photo_sq_height, row.teaser_photo_sq_size);

            return category;
        }


        Photo BuildPhoto(dynamic row)
        {
            var photo = new Photo();

            photo.Id = (int) row.id;
            photo.CategoryId = (short) row.category_id;
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
}
