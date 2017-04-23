using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Maw.Domain.Videos;


namespace Maw.Data
{
	public class VideoRepository
        : Repository, IVideoRepository
	{
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
                return conn.QueryAsync<Category, VideoInfo, Category>(
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
                    (category, videoInfo) => {
                        category.TeaserThumbnail = videoInfo;
                        return category; 
                    },
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
                IEnumerable<dynamic> result = await conn.QueryAsync(
                    @"SELECT *
                        FROM video.video v
                       INNER JOIN video.category c ON v.category_id = c.id
                       WHERE (1 = @allowPrivate OR v.is_private = FALSE)
                         AND c.id = @categoryId;",
                    new { 
                        allowPrivate = allowPrivate ? 1 : 0,
                        categoryId = categoryId
                    }
                );

                return result.Select(x => new Video {
                    Id = x.id,
                    Duration = x.duration,
                    Category = new Category {
                        Id = x.category_id,
                        Year = x.year,
                        Name = x.name,
                        TeaserThumbnail = new VideoInfo {
                            Path = x.teaser_image_path,
                            Width = x.teaser_image_width,
                            Height = x.teaser_image_height
                        }
                    },
                    ScaledVideo = new VideoInfo {
                        Path = x.scaled_path,
                        Width = x.scaled_width,
                        Height = x.scaled_height
                    },
                    FullsizeVideo = new VideoInfo {
                        Path = x.full_path,
                        Width = x.full_width,
                        Height = x.full_height
                    },
                    ThumbnailVideo = new VideoInfo {
                        Path = x.thumb_path,
                        Width = x.thumb_width,
                        Height = x.thumb_height
                    }
                });
            });
		}
		
		
		public Task<Video> GetVideoAsync(short id, bool allowPrivate)
		{
            return RunAsync(async conn => {
                var result = await conn.QueryAsync<Video, Category, Video>(
                    @"SELECT *
                        FROM video.video v
                       INNER JOIN video.category c ON v.category_id = c.id
                       WHERE (1 = @allowPrivate OR v.is_private = FALSE)
                         AND v.id = @id;",
                    (vid, cat) => {
                        vid.Category = cat;
                        return vid;
                    },
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
                var result = await conn.QueryAsync<Category, VideoInfo, Category>(
                    @"SELECT id,
                             year,
                             name,
                             teaser_image_path AS path,
                             teaser_image_width AS width,
                             teaser_image_height AS height
                        FROM video.category
                       WHERE (1 = @allowPrivate OR is_private = FALSE)
                         AND id = @id;",
                    (category, videoInfo) => {
                        category.TeaserThumbnail = videoInfo;
                        return category; 
                    },
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
	}
}
