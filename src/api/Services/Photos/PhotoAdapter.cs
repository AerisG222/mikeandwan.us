using System;
using System.Collections.Generic;
using System.Linq;
using Maw.Domain.Photos;


namespace MawApi.Services.Photos
{
    public class PhotoAdapter
    {
        readonly PhotoUrlBuilderService _urlSvc;
        readonly PhotoMultimediaInfoAdapter _adapter;


        public PhotoAdapter(
            PhotoUrlBuilderService urlSvc,
            PhotoMultimediaInfoAdapter adapter)
        {
            _urlSvc = urlSvc ?? throw new ArgumentNullException(nameof(urlSvc));
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }


        public MawApi.ViewModels.Photos.PhotoViewModel Adapt(Photo p)
        {
            if(p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            return new MawApi.ViewModels.Photos.PhotoViewModel {
                Id = p.Id,
                CategoryId = p.CategoryId,
                CreateDate = DateTime.MinValue,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                ImageXsSq = _adapter.Adapt(p.XsSqInfo, p.Id, "xs_sq"),
                ImageXs = _adapter.Adapt(p.XsInfo, p.Id, "xs"),
                ImageSm = _adapter.Adapt(p.SmInfo, p.Id, "sm"),
                ImageMd = _adapter.Adapt(p.MdInfo, p.Id, "md"),
                ImageLg = _adapter.Adapt(p.LgInfo, p.Id, "lg"),
                ImagePrt = _adapter.Adapt(p.PrtInfo, p.Id, "prt"),
                ImageSrc = _adapter.Adapt(p.SrcInfo, p.Id, "src"),
                Self = _urlSvc.GetPhotoUrl(p.Id),
                CategoryLink = _urlSvc.GetCategoryUrl(p.CategoryId),
                CommentsLink = _urlSvc.GetCommentsUrl(p.Id),
                ExifLink = _urlSvc.GetExifUrl(p.Id),
                RatingLink = _urlSvc.GetRatingUrl(p.Id)
            };
        }


        public IEnumerable<MawApi.ViewModels.Photos.PhotoViewModel> Adapt(IEnumerable<Photo> photos)
        {
            return photos.Select(p => Adapt(p));
        }
    }
}
