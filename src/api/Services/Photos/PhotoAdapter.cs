using System;
using System.Collections.Generic;
using System.Linq;
using Maw.Domain.Photos;


namespace MawApi.Services.Photos
{
    public class PhotoAdapter
    {
        readonly PhotoUrlBuilderService _urlSvc;
        readonly MultimediaInfoAdapter _adapter;


        public PhotoAdapter(
            PhotoUrlBuilderService urlSvc,
            MultimediaInfoAdapter adapter)
        {
            _urlSvc = urlSvc ?? throw new ArgumentNullException(nameof(urlSvc));
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }


        public MawApi.ViewModels.Photos.PhotoViewModel Adapt(Photo p)
        {
            return new MawApi.ViewModels.Photos.PhotoViewModel {
                Id = p.Id,
                CategoryId = p.CategoryId,
                CreateDate = DateTime.MinValue,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                ImageXsSq = _adapter.Adapt(p.XsSqInfo),
                ImageXs = _adapter.Adapt(p.XsInfo),
                ImageSm = _adapter.Adapt(p.SmInfo),
                ImageMd = _adapter.Adapt(p.MdInfo),
                ImageLg = _adapter.Adapt(p.LgInfo),
                ImagePrt = _adapter.Adapt(p.PrtInfo),
                ImageSrc = _adapter.Adapt(p.SrcInfo),
                Self = _urlSvc.GetPhotoUrl(p.Id),
                CategoryLink = _urlSvc.GetPhotoCategoryUrl(p.CategoryId)
            };
        }


        public IEnumerable<MawApi.ViewModels.Photos.PhotoViewModel> Adapt(IEnumerable<Photo> photos)
        {
            return photos.Select(p => Adapt(p));
        }


        public MawApi.ViewModels.Photos.PhotoViewModel Adapt(PhotoAndCategory p)
        {
            return Adapt(p.Photo);
        }


        public IEnumerable<MawApi.ViewModels.Photos.PhotoViewModel> Adapt(IEnumerable<PhotoAndCategory> photos)
        {
            return photos.Select(p => Adapt(p.Photo));
        }
    }
}
