using System;
using System.Collections.Generic;
using System.Linq;
using Maw.Domain.Videos;
using MawApi.ViewModels.Videos;


namespace MawApi.Services.Videos
{
    public class VideoCategoryAdapter
    {
        readonly VideoUrlBuilderService _urlSvc;
        readonly MultimediaInfoAdapter _adapter;


        public VideoCategoryAdapter(
            VideoUrlBuilderService urlSvc,
            MultimediaInfoAdapter adapter)
        {
            _urlSvc = urlSvc ?? throw new ArgumentNullException(nameof(urlSvc));
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }


        public VideoCategoryViewModel Adapt(Category c)
        {
            return new VideoCategoryViewModel {
                Id = c.Id,
                Name = c.Name,
                Year = c.Year,
                CreateDate = DateTime.MinValue,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
                VideoCount = c.VideoCount,
                TotalDuration = c.TotalDuration,
                TotalSizeThumbnail = c.TotalSizeThumbnail,
                TotalSizeThumbnailSq = c.TotalSizeThumbnailSq,
                TotalSizeScaled = c.TotalSizeScaled,
                TotalSizeFull = c.TotalSizeFull,
                TotalSizeRaw = c.TotalSizeRaw,
                TotalSize = c.TotalSize,
                TeaserImage = _adapter.Adapt(c.TeaserImage),
                TeaserImageSq = _adapter.Adapt(c.TeaserImageSq),
                Self = _urlSvc.GetPhotoCategoryUrl(c.Id)
            };
        }


        public IEnumerable<MawApi.ViewModels.Videos.VideoCategoryViewModel> Adapt(IEnumerable<Category> categories)
        {
            return categories.Select(c => Adapt(c));
        }
    }
}