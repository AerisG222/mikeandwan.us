using System;

namespace MawApi.Services.Videos;

public class VideoUrlBuilderService
{
    readonly UrlBuilderService _urlSvc;

    public VideoUrlBuilderService(UrlBuilderService urlSvc)
    {
        _urlSvc = urlSvc ?? throw new ArgumentNullException(nameof(urlSvc));
    }

    public string GetCategoryUrl(short categoryId)
    {
        return _urlSvc.BuildApiUrl($"video-categories/{categoryId}");
    }

    public string GetVideosUrl(short categoryId)
    {
        return _urlSvc.BuildApiUrl($"video-categories/{categoryId}/photos");
    }

    public string GetVideoUrl(int videoId)
    {
        return _urlSvc.BuildApiUrl($"videos/{videoId}");
    }

    public string GetCommentsUrl(int videoId)
    {
        return _urlSvc.BuildApiUrl($"videos/{videoId}/comments");
    }

    public string GetRatingUrl(int videoId)
    {
        return _urlSvc.BuildApiUrl($"videos/{videoId}/rating");
    }

    public string GetMediaUrl(string relativePath)
    {
        return _urlSvc.BuildWwwUrl(relativePath);
    }
}
