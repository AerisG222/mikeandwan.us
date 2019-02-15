using System;
using Maw.Domain.Photos;
using MawApi.ViewModels;
using MawApi.ViewModels.Photos;


namespace MawApi.Services.Photos
{
    public class ImageAdapter
    {
        readonly PhotoUrlBuilderService _urlSvc;


        public ImageAdapter(PhotoUrlBuilderService urlSvc)
        {
            _urlSvc = urlSvc ?? throw new ArgumentNullException(nameof(urlSvc));
        }


        public MultimediaAsset Adapt(PhotoInfo info)
        {
            return new MultimediaAsset {
                Height = info.Height,
                Width = info.Width,
                Url = _urlSvc.GetImageUrl(info.Path),
                Size = 0
            };
        }
    }
}
