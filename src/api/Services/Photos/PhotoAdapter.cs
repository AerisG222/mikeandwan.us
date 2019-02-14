using System;
using System.Collections.Generic;
using System.Linq;
using Maw.Domain.Photos;


namespace MawApi.Services.Photos
{
    public class PhotoAdapter
    {
        readonly PhotoUrlBuilderService _urlSvc;
        readonly ImageAdapter _imgAdapter;


        public PhotoAdapter(
            PhotoUrlBuilderService urlSvc,
            ImageAdapter imageAdapter)
        {
            _urlSvc = urlSvc ?? throw new ArgumentNullException(nameof(urlSvc));
            _imgAdapter = imageAdapter ?? throw new ArgumentNullException(nameof(imageAdapter));
        }


        public MawApi.ViewModels.Photos.PhotoViewModel Adapt(Photo p)
        {
            return new MawApi.ViewModels.Photos.PhotoViewModel {
                Id = p.Id,
                CategoryId = p.CategoryId,
                TakenDateUtc = DateTime.MinValue,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                ImageXs = _imgAdapter.Adapt(p.XsInfo),
                ImageSm = _imgAdapter.Adapt(p.SmInfo),
                ImageMd = _imgAdapter.Adapt(p.MdInfo),
                ImageLg = _imgAdapter.Adapt(p.LgInfo),
                ImagePrt = _imgAdapter.Adapt(p.PrtInfo),
                ImageRaw = null,
                Self = _urlSvc.GetPhotoUrl(p.Id),
                CategoryLink = _urlSvc.GetPhotoCategoryUrl(p.CategoryId)
            };
        }


        public IEnumerable<MawApi.ViewModels.Photos.PhotoViewModel> Adapt(IEnumerable<Photo> photos)
        {
            return photos.Select(p => Adapt(p));
        }
    }
}
