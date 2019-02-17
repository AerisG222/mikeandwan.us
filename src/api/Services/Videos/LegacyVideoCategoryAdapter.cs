using System;
using System.Collections.Generic;
using System.Linq;


namespace MawApi.Services.Videos
{
    public class LegacyVideoCategoryAdapter
    {
        readonly LegacyMultimediaInfoAdapter _adapter;


        public LegacyVideoCategoryAdapter(
            LegacyMultimediaInfoAdapter adapter)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }


        public MawApi.ViewModels.LegacyVideos.Category Adapt(Maw.Domain.Videos.Category c)
        {
            return new MawApi.ViewModels.LegacyVideos.Category {
                Id = c.Id,
                Name = c.Name,
                Year = c.Year,
                TeaserThumbnail = _adapter.Adapt(c.TeaserImage)
            };
        }


        public IEnumerable<MawApi.ViewModels.LegacyVideos.Category> Adapt(IEnumerable< Maw.Domain.Videos.Category> categories)
        {
            return categories.Select(c => Adapt(c));
        }
    }
}
