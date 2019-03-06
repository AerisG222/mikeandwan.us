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


        public string GetCategoryUrl(short categoryId)
        {
            return _urlSvc.BuildApiUrl($"photo-categories/{categoryId}");
        }


        public string GetPhotosUrl(short categoryId)
        {
            return _urlSvc.BuildApiUrl($"photo-categories/{categoryId}/photos");
        }


        public string GetCategoryDownloadUrl(short categoryId)
        {
            return _urlSvc.BuildWwwUrl($"/photos/download-category/{categoryId}");
        }


        public string GetPhotoUrl(int photoId)
        {
            return _urlSvc.BuildApiUrl($"photos/{photoId}");
        }


        public string GetCommentsUrl(int photoId)
        {
            return _urlSvc.BuildApiUrl($"photos/{photoId}/comments");
        }


        public string GetExifUrl(int photoId)
        {
            return _urlSvc.BuildApiUrl($"photos/{photoId}/exif");
        }


        public string GetRatingUrl(int photoId)
        {
            return _urlSvc.BuildApiUrl($"photos/{photoId}/rating");
        }


        public string GetImageUrl(string relativePath)
        {
            return _urlSvc.BuildWwwUrl(relativePath);
        }


        public string GetImageDownloadUrl(int photoId, string size)
        {
            return _urlSvc.BuildWwwUrl($"photos/download/{photoId}/{size}");
        }
    }
}
