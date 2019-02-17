using System;


namespace MawApi.Services.Photos
{
    public class PhotoUrlBuilderService
    {
        readonly UrlBuilderService _urlSvc;


        public PhotoUrlBuilderService(UrlBuilderService urlSvc)
        {
            _urlSvc = urlSvc ?? throw new ArgumentNullException(nameof(urlSvc));
        }


        public string GetPhotoCategoryUrl(short categoryId)
        {
            return _urlSvc.BuildApiUrl($"photo-categories/{categoryId}");
        }


        public string GetPhotoUrl(int photoId)
        {
            return _urlSvc.BuildApiUrl($"photos/{photoId}");
        }


        public string GetImageUrl(string relativePath)
        {
            return _urlSvc.BuildWwwUrl(relativePath);
        }
    }
}
