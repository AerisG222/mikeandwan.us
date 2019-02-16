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


		public Task<IEnumerable<Category>> GetCategoriesAsync(short year, bool allowPrivate)
		{
            return RunAsync(conn => {
                return conn.QueryAsync<Category>(
                    @"SELECT id,
                             year,
                             name,
                             teaser_image_path AS path,
                             teaser_image_width AS width,
                             teaser_image_height AS height
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
                    @"SELECT v.*
                        FROM video.video v
                       WHERE (1 = @allowPrivate OR v.is_private = FALSE)
                         AND c.id = @categoryId;",
                    VIDEO_PROJECTION_TYPES,
                    (objects) => AssembleVideo(objects),
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        categoryId = categoryId
                    }
                );
            });
		}


		public Task<Video> GetVideoAsync(short id, bool allowPrivate)
		{
            return RunAsync(async conn => {
                var result = await conn.QueryAsync<Video>(
                    @"SELECT *
                        FROM video.video v
                       WHERE (1 = @allowPrivate OR v.is_private = FALSE)
                         AND v.id = @id;",
                    VIDEO_PROJECTION_TYPES,
                    (objects) => AssembleVideo(objects),
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        id = id
                    }
                ).ConfigureAwait(false);

                return result.FirstOrDefault();
            });
		}


        public Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate)
        {
            return RunAsync(async conn => {
                var result = await conn.QueryAsync<Category>(
                    @"SELECT id,
                             year,
                             name,
                             teaser_image_path AS path,
                             teaser_image_width AS width,
                             teaser_image_height AS height
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
