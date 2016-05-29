using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Maw.Data.EntityFramework.Videos;
using Maw.Domain.Videos;


namespace Maw.Data
{
	public class VideoRepository
        : IVideoRepository
	{
		readonly VideoContext _ctx;


        public VideoRepository(VideoContext context)
        {
			if(context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

            _ctx = context;
        }


		public Task<List<short>> GetYearsAsync(bool allowPrivate)
		{
            return _ctx.category
                .Where(x => allowPrivate || !x.is_private)
                .Select(x => x.year)
                .Distinct()
                .OrderByDescending(x => x)
				.ToListAsync();
		}
		
		
		public async Task<List<Category>> GetCategoriesAsync(short year, bool allowPrivate)
		{
			var cats = await _ctx.category
                .Where(x => x.year == year && (allowPrivate || !x.is_private))
                .OrderBy(x => x.id)
				.ToListAsync();

			return cats
                .Select(x => BuildVideoCategory(x))
				.ToList();
		}
		

		public async Task<List<Video>> GetVideosInCategoryAsync(short categoryId, bool allowPrivate)
		{
			var vids = await _ctx.video
                .Where(x => x.category_id == categoryId && (allowPrivate || !x.is_private))
                .Join(_ctx.category, vid => vid.category_id, cat => cat.id, (vid, cat) => new { Video = vid, Category = cat })
				.ToListAsync();

			return vids.Select(x => BuildVideo(x.Video, x.Category))
				.ToList();
		}
		
		
		public async Task<Video> GetVideoAsync(short id, bool allowPrivate)
		{
			var video = await _ctx.video
                .Where(x => x.id == id && (allowPrivate || !x.is_private))
                .Join(_ctx.category, vid => vid.category_id, cat => cat.id, (vid, cat) => new { Video = vid, Category = cat })
				.SingleAsync();

			return BuildVideo(video.Video, video.Category);
		}
		
		
		public async Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate)
		{
			var cat = await _ctx.category
                .Where(x => x.id == categoryId && (allowPrivate || !x.is_private))
				.SingleAsync();

			return BuildVideoCategory(cat);
		}
		

        Category BuildVideoCategory(category x)
        {
            return new Category {
                Id = x.id,
                Name = x.name,
                Year = x.year,
                TeaserThumbnail = new VideoInfo()
                {
                    Path = x.teaser_image_path,
					Height = (short)x.teaser_image_height,
					Width = (short)x.teaser_image_width
                }
            };
        }


        Video BuildVideo(video x, category y)
        {
            return new Video {
                Id = x.id,
				ThumbnailVideo = new VideoInfo()
                {
                    Height = x.thumb_height,
                    Width = x.thumb_width,
                    Path = x.thumb_path
                },
				FullsizeVideo = new VideoInfo()
                {
                    Height = x.full_height,
                    Width = x.full_width,
                    Path = x.full_path
                },
				ScaledVideo = new VideoInfo()
                {
                    Height = x.scaled_height,
                    Width = x.scaled_width,
                    Path = x.scaled_path
                },
                Category = new Category { 
                    Id = y.id,
                    Year = y.year,
                    Name = y.name
                }
            };
        }
	}
}
