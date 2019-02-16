using System;
using Maw.Domain;
using Maw.Domain.Photos;
using MawApi.ViewModels;
using MawApi.ViewModels.LegacyPhotos;
using MawApi.ViewModels.Photos;


namespace MawApi.Services
{
    public class LegacyMultimediaInfoAdapter
    {
        public PhotoInfo Adapt(MultimediaInfo info)
        {
            if(info == null)
            {
                return new PhotoInfo();
            }

            return new PhotoInfo {
                Height = info.Height,
                Width = info.Width,
                Path = info.Path
            };
        }
    }
}
