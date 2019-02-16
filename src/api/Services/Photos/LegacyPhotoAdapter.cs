using System;
using System.Collections.Generic;
using System.Linq;
using Maw.Domain.Photos;


namespace MawApi.Services.Photos
{
    public class LegacyPhotoAdapter
    {
        readonly LegacyMultimediaInfoAdapter _adapter;


        public LegacyPhotoAdapter(
            LegacyMultimediaInfoAdapter adapter)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }


        public MawApi.ViewModels.LegacyPhotos.Photo Adapt(Photo p)
        {
            return new MawApi.ViewModels.LegacyPhotos.Photo {
                Id = p.Id,
                CategoryId = p.CategoryId,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                XsInfo = _adapter.Adapt(p.XsInfo),
                SmInfo = _adapter.Adapt(p.SmInfo),
                MdInfo = _adapter.Adapt(p.MdInfo),
                LgInfo = _adapter.Adapt(p.LgInfo),
                PrtInfo = _adapter.Adapt(p.PrtInfo)
            };
        }


        public IEnumerable<MawApi.ViewModels.LegacyPhotos.Photo> Adapt(IEnumerable<Photo> photos)
        {
            return photos.Select(p => Adapt(p));
        }
    }
}
