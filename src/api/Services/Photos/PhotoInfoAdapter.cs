using System;
using Maw.Domain.Photos;
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


        public Image Adapt(PhotoInfo info)
        {
            return new Image {
                Height = info.Height,
                Width = info.Width,
                Url = _urlSvc.GetImageUrl(info.Path),
                ByteCount = 0
            };
        }
    }
}
