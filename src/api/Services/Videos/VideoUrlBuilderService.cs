using System;


namespace MawApi.Services.Videos
{
    public class VideoUrlBuilderService
    {
        readonly UrlBuilderService _urlSvc;


        public VideoUrlBuilderService(UrlBuilderService urlSvc)
        {
            _urlSvc = urlSvc ?? throw new ArgumentNullException(nameof(urlSvc));
        }


        public string GetPhotoCategoryUrl(short categoryId)
        {
            return _urlSvc.BuildApiUrl($"video-categories/{categoryId}");
        }


        public string GetPhotoUrl(int photoId)
        {
            return _urlSvc.BuildApiUrl($"videos/{photoId}");
        }


        public string GetMediaUrl(string relativePath)
        {
            return _urlSvc.BuildWwwUrl(relativePath);
        }
    }
}
