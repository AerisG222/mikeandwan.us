using System;
using System.Collections.Generic;
using System.Linq;
using Maw.Domain.Photos;
using MawApi.ViewModels.Photos;


namespace MawApi.Services.Photos
{
    public class PhotoCategoryAdapter
    {
        readonly PhotoUrlBuilderService _urlSvc;
        readonly MultimediaInfoAdapter _adapter;


        public PhotoCategoryAdapter(
            PhotoUrlBuilderService urlSvc,
            MultimediaInfoAdapter adapter)
        {
            _urlSvc = urlSvc ?? throw new ArgumentNullException(nameof(urlSvc));
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }


        public PhotoCategoryViewModel Adapt(Category c)
        {
            return new PhotoCategoryViewModel {
                Id = c.Id,
                Name = c.Name,
                Year = c.Year,
                CreateDate = DateTime.MinValue,
                Latitude = 0,
                Longitude = 0,
                PhotoCount = c.PhotoCount,
                TeaserImage = _adapter.Adapt(c.TeaserImage),
                TeaserImageSq = _adapter.Adapt(c.TeaserImageSq),
                Self = _urlSvc.GetPhotoCategoryUrl(c.Id)
            };
        }


        public IEnumerable<MawApi.ViewModels.Photos.PhotoCategoryViewModel> Adapt(IEnumerable<Category> categories)
        {
            return categories.Select(c => Adapt(c));
        }
    }
}
