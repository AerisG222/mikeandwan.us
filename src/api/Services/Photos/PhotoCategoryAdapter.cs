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
        readonly ImageAdapter _imgAdapter;


        public PhotoCategoryAdapter(
            PhotoUrlBuilderService urlSvc,
            ImageAdapter imageAdapter)
        {
            _urlSvc = urlSvc ?? throw new ArgumentNullException(nameof(urlSvc));
            _imgAdapter = imageAdapter ?? throw new ArgumentNullException(nameof(imageAdapter));
        }


        public PhotoCategoryViewModel Adapt(Category c)
        {
            return new PhotoCategoryViewModel {
                Id = c.Id,
                Name = c.Name,
                Year = c.Year,
                TakenDateUtc = DateTime.MinValue,
                Latitude = 0,
                Longitude = 0,
                PhotoCount = c.PhotoCount,
                TeaserImage = _imgAdapter.Adapt(c.TeaserPhotoInfo),
                Self = _urlSvc.GetPhotoCategoryUrl(c.Id)
            };
        }


        public IEnumerable<MawApi.ViewModels.Photos.PhotoCategoryViewModel> Adapt(IEnumerable<Category> categories)
        {
            return categories.Select(c => Adapt(c));
        }
    }
}
