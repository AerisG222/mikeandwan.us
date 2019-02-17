using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Maw.Domain;
using Maw.Domain.Videos;


namespace Maw.Data
{
	public class VideoRepository
        : Repository, IVideoRepository
	{
        static readonly Type[] CATEGORY_PROJECTION_TYPES = new Type[] {
            typeof(Category),
            typeof(MultimediaInfo),
            typeof(MultimediaInfo)
        };

        static readonly Type[] VIDEO_PROJECTION_TYPES = new Type[] {
            typeof(Video),
            typeof(MultimediaInfo),
            typeof(MultimediaInfo),
            typeof(MultimediaInfo),
            typeof(MultimediaInfo),
            typeof(MultimediaInfo)
        };

        const string CATEGORY_PROJECTION = @"
            id,
            year,
            name,
            create_date,
            CASE WHEN gps_latitude_ref_id = 'S' THEN -1.0 * gps_latitude
                 ELSE gps_latitude
                  END AS latitude,
            CASE WHEN gps_longitude_ref_id = 'W' THEN -1.0 * gps_longitude
                 ELSE gps_longitude
                  END AS longitude,
            video_count,
            total_duration,
            total_size_thumb,
            total_size_thumb_sq,
            total_size_scaled,
            total_size_full,
            total_size_raw,
            COALESCE(total_size_thumb, 0) +
                COALESCE(total_size_thumb_sq, 0) +
                COALESCE(total_size_scaled, 0) +
                COALESCE(total_size_full, 0) +
                COALESCE(total_size_raw, 0) AS total_size,
            teaser_image_path AS path,
            teaser_image_width AS width,
            teaser_image_height AS height,
            teaser_image_size AS size,
            teaser_image_sq_path AS path,
            teaser_image_sq_width AS width,
            teaser_image_sq_height AS height,
            teaser_image_sq_size AS size";

        const string VIDEO_PROJECTION = @"
            id,
            category_id,
            duration,
            create_date,
            gps_latitude AS latitude,
            gps_longitude AS longitude,
            thumb_path AS path,
            thumb_height AS height,
            thumb_width AS width,
            thumb_size AS size,
            thumb_sq_path AS path,
            thumb_sq_height AS height,
            thumb_sq_width AS width,
            thumb_sq_size AS size,
            scaled_path AS path,
            scaled_height AS height,
            scaled_width AS width,
            scaled_size AS size,
            full_path AS path,
            full_height AS height,
            full_width AS width,
            full_size AS size,
            raw_path AS path,
            raw_height AS height,
            raw_width AS width,
            raw_size AS size";

        public VideoRepository(string connectionString)
            : base(connectionString)
        {

        }


		public Task<IEnumerable<short>> GetYearsAsync(bool allowPrivate)
		{
            return RunAsync(conn => {
                return conn.QueryAsync<short>(
                    @"SELECT DISTINCT year
                        FROM video.category
                       WHERE (1 = @allowPrivate OR is_private = FALSE)
                       ORDER BY year DESC;",
                    new { allowPrivate = allowPrivate ? 1 : 0 }
                );
            });
		}


        public Task<IEnumerable<Category>> GetAllCategoriesAsync(bool allowPrivate)
		{
            return RunAsync(conn => {
                return conn.QueryAsync<Category>(
                    $@"SELECT {CATEGORY_PROJECTION}
                        FROM video.category
                       WHERE (1 = @allowPrivate OR is_private = FALSE)
                       ORDER BY id;",
                    CATEGORY_PROJECTION_TYPES,
                    (objects) => AssembleCategory(objects),
                    new {
                        allowPrivate = allowPrivate ? 1 : 0
                    },
                    splitOn: "path"
                );
            });
		}


		public Task<IEnumerable<Category>> GetCategoriesAsync(short year, bool allowPrivate)
		{
            return RunAsync(conn => {
                return conn.QueryAsync<Category>(
                    $@"SELECT {CATEGORY_PROJECTION}
                        FROM video.category
                       WHERE (1 = @allowPrivate OR is_private = FALSE)
                         AND year = @year
                       ORDER BY id;",
                    CATEGORY_PROJECTION_TYPES,
                    (objects) => AssembleCategory(objects),
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        year = year
                    },
                    splitOn: "path"
                );
            });
		}


		public Task<IEnumerable<Video>> GetVideosInCategoryAsync(short categoryId, bool allowPrivate)
		{
            return RunAsync(async conn => {
                return await conn.QueryAsync<Video>(
                    $@"SELECT {VIDEO_PROJECTION}
                         FROM video.video
                        WHERE (1 = @allowPrivate OR is_private = FALSE)
                          AND category_id = @categoryId;",
                    VIDEO_PROJECTION_TYPES,
                    (objects) => AssembleVideo(objects),
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        categoryId = categoryId
                    },
                    splitOn: "path"
                );
            });
		}


		public Task<Video> GetVideoAsync(short id, bool allowPrivate)
		{
            return RunAsync(async conn => {
                var result = await conn.QueryAsync<Video>(
                    $@"SELECT {VIDEO_PROJECTION}
                         FROM video.video
                        WHERE (1 = @allowPrivate OR is_private = FALSE)
                          AND id = @id;",
                    VIDEO_PROJECTION_TYPES,
                    (objects) => AssembleVideo(objects),
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        id = id
                    },
                    splitOn: "path"
                ).ConfigureAwait(false);

                return result.FirstOrDefault();
            });
		}


        public Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate)
        {
            return RunAsync(async conn => {
                var result = await conn.QueryAsync<Category>(
                    $@"SELECT {CATEGORY_PROJECTION}
                         FROM video.category
                        WHERE (1 = @allowPrivate OR is_private = FALSE)
                          AND id = @id;",
                    CATEGORY_PROJECTION_TYPES,
                    (objects) => AssembleCategory(objects),
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        id = categoryId
                    },
                    splitOn: "path"
                );

                if(result == null || result.Count() == 0)
                {
                    return null;
                }

                return result.First();
            });
        }


        public Task<IEnumerable<Comment>> GetCommentsAsync(int videoId)
		{
            return RunAsync(conn => {
                return conn.QueryAsync<Comment>(
                    @"SELECT entry_date,
                             message AS comment_text,
                             u.username
                        FROM video.comment c
                       INNER JOIN maw.user u ON c.user_id = u.id
                       WHERE c.video_id = @videoId
                       ORDER by entry_date DESC;",
                    new { videoId = videoId }
                );
            });
		}


		public Task<Rating> GetRatingsAsync(int videoId, string username)
		{
            return RunAsync(conn => {
                return conn.QuerySingleOrDefaultAsync<Rating>(
                    @"SELECT (SELECT AVG(score)
                                FROM video.rating
                               WHERE video_id = @videoId
                             ) AS average_rating,
                             (SELECT AVG(score)
                                FROM video.rating
                               WHERE video_id = @videoId
                                 AND user_id = (SELECT id
                                                  FROM maw.user
                                                 WHERE username = @username
                                               )
                             ) AS user_rating;",
                    new {
                        videoId = videoId,
                        username = username?.ToLower()
                    }
                );
            });
		}


		public Task<int> InsertCommentAsync(int videoId, string username, string comment)
        {
            return RunAsync(conn => {
                return conn.ExecuteAsync(
                    @"INSERT INTO video.comment
                           (
                             user_id,
                             video_id,
                             message,
                             entry_date
                           )
                      VALUES
                           (
                             (SELECT id FROM maw.user WHERE username = @username),
                             @videoId,
                             @message,
                             @entryDate
                           );",
                    new {
                        username = username.ToLower(),
                        videoId = videoId,
                        message = comment,
                        entryDate = DateTime.Now
                    }
                );
            });
        }


		public Task<float?> SaveRatingAsync(int videoId, string username, byte rating)
        {
            return RunAsync(async conn => {
                var result = await conn.ExecuteAsync(
                    @"INSERT INTO video.rating
                           (
                             video_id,
                             user_id,
                             score
                           )
                      VALUES
                           (
                             @videoId,
                             (SELECT id
                                FROM maw.user
                               WHERE username = @username
                             ),
                             @score
                           )
                        ON CONFLICT (video_id, user_id)
                        DO UPDATE
                       SET score = @score;",
                    new {
                        videoId = videoId,
                        username = username.ToLower(),
                        score = rating
                    }
                ).ConfigureAwait(false);

                return (await GetRatingsAsync(videoId, username).ConfigureAwait(false))?.AverageRating;
            });
        }


		public Task<float?> RemoveRatingAsync(int videoId, string username)
		{
            return RunAsync(async conn => {
                var result = await conn.ExecuteAsync(
                    @"DELETE FROM video.rating
                       WHERE video_id = @videoId
                         AND user_id = (SELECT id
                                          FROM maw.user
                                         WHERE username = @username
                                       );",
                    new {
                        videoId = videoId,
                        username = username.ToLower()
                    }
                ).ConfigureAwait(false);

                return (await GetRatingsAsync(videoId, username).ConfigureAwait(false))?.AverageRating;
            });
		}


        Category AssembleCategory(object[] objects)
        {
            var category = (Category) objects[0];

            category.TeaserImage = (MultimediaInfo) objects[1];
            category.TeaserImageSq = (MultimediaInfo) objects[2];

            return category;
        }


        Video AssembleVideo(object[] objects)
        {
            var video = (Video) objects[0];

            video.Thumbnail = (MultimediaInfo) objects[1];
            video.ThumbnailSq = (MultimediaInfo) objects[2];
            video.VideoScaled = (MultimediaInfo) objects[3];
            video.VideoFull = (MultimediaInfo) objects[4];
            video.VideoRaw = (MultimediaInfo) objects[5];

            return video;
        }
	}
}
