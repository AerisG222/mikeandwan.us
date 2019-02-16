using System;
using System.Collections.Generic;
using System.Linq;
using Maw.Domain.Photos;
using MawApi.ViewModels.Photos;


namespace MawApi.Services.Photos
{
    public class LegacyPhotoCategoryAdapter
    {
        readonly LegacyMultimediaInfoAdapter _adapter;


        public LegacyPhotoCategoryAdapter(
            LegacyMultimediaInfoAdapter adapter)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }


        public MawApi.ViewModels.LegacyPhotos.Category Adapt(Category c)
        {
            return new MawApi.ViewModels.LegacyPhotos.Category {
                Id = c.Id,
                Name = c.Name,
                Year = c.Year,
                HasGpsData = c.Latitude != null,
                TeaserPhotoInfo = _adapter.Adapt(c.TeaserImage)
            };
        }


        public IEnumerable<MawApi.ViewModels.LegacyPhotos.Category> Adapt(IEnumerable<Category> categories)
        {
            return categories.Select(c => Adapt(c));
        }
    }
}
