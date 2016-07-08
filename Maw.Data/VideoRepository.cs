using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Maw.Data.EntityFramework.Videos;
using D = Maw.Domain.Videos;


namespace Maw.Data
{
	public class VideoRepository
        : D.IVideoRepository
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
            return _ctx.Category
                .Where(x => allowPrivate || !x.IsPrivate)
                .Select(x => x.Year)
                .Distinct()
                .OrderByDescending(x => x)
				.ToListAsync();
		}
		
		
		public async Task<List<D.Category>> GetCategoriesAsync(short year, bool allowPrivate)
		{
			var cats = await _ctx.Category
                .Where(x => x.Year == year && (allowPrivate || !x.IsPrivate))
                .OrderBy(x => x.Id)
				.ToListAsync();

			return cats
                .Select(x => BuildVideoCategory(x))
				.ToList();
		}
		

		public async Task<List<D.Video>> GetVideosInCategoryAsync(short categoryId, bool allowPrivate)
		{
			var vids = await _ctx.Video
                .Where(x => x.CategoryId == categoryId && (allowPrivate || !x.IsPrivate))
                .Join(_ctx.Category, vid => vid.CategoryId, cat => cat.Id, (vid, cat) => new { Video = vid, Category = cat })
				.ToListAsync();

			return vids.Select(x => BuildVideo(x.Video, x.Category))
				.ToList();
		}
		
		
		public async Task<D.Video> GetVideoAsync(short id, bool allowPrivate)
		{
			var video = await _ctx.Video
                .Where(x => x.Id == id && (allowPrivate || !x.IsPrivate))
                .Join(_ctx.Category, vid => vid.CategoryId, cat => cat.Id, (vid, cat) => new { Video = vid, Category = cat })
				.SingleAsync();

			return BuildVideo(video.Video, video.Category);
		}
		
		
		public async Task<D.Category> GetCategoryAsync(short categoryId, bool allowPrivate)
		{
			var cat = await _ctx.Category
                .Where(x => x.Id == categoryId && (allowPrivate || !x.IsPrivate))
				.SingleAsync();

			return BuildVideoCategory(cat);
		}
		

        D.Category BuildVideoCategory(Category x)
        {
            return new D.Category {
                Id = x.Id,
                Name = x.Name,
                Year = x.Year,
                TeaserThumbnail = new D.VideoInfo
                {
                    Path = x.TeaserImagePath,
					Height = (short)x.TeaserImageHeight,
					Width = (short)x.TeaserImageWidth
                }
            };
        }


        D.Video BuildVideo(Video x, Category y)
        {
            return new D.Video {
                Id = x.Id,
				ThumbnailVideo = new D.VideoInfo
                {
                    Height = x.ThumbHeight,
                    Width = x.ThumbWidth,
                    Path = x.ThumbPath
                },
				FullsizeVideo = new D.VideoInfo
                {
                    Height = x.FullHeight,
                    Width = x.FullWidth,
                    Path = x.FullPath
                },
				ScaledVideo = new D.VideoInfo
                {
                    Height = x.ScaledHeight,
                    Width = x.ScaledWidth,
                    Path = x.ScaledPath
                },
                Category = new D.Category 
                { 
                    Id = y.Id,
                    Year = y.Year,
                    Name = y.Name
                }
            };
        }
	}
}
