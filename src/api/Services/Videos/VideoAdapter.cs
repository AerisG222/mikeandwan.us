using Maw.Domain.Models.Videos;

namespace MawApi.Services.Videos;

public class VideoAdapter
{
    readonly VideoUrlBuilderService _urlSvc;
    readonly MultimediaInfoAdapter _adapter;

    public VideoAdapter(
        VideoUrlBuilderService urlSvc,
        MultimediaInfoAdapter adapter)
    {
        _urlSvc = urlSvc ?? throw new ArgumentNullException(nameof(urlSvc));
        _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
    }

    public MawApi.ViewModels.Videos.VideoViewModel Adapt(Video v)
    {
        ArgumentNullException.ThrowIfNull(v);

        return new MawApi.ViewModels.Videos.VideoViewModel {
            Id = v.Id,
            CategoryId = v.CategoryId,
            CreateDate = v.CreateDate ?? DateTime.MinValue,
            Latitude = v.Latitude,
            Longitude = v.Longitude,
            Duration = v.Duration,
            Thumbnail = _adapter.Adapt(v.Thumbnail),
            ThumbnailSq = _adapter.Adapt(v.ThumbnailSq),
            VideoScaled = _adapter.Adapt(v.VideoScaled),
            VideoFull = _adapter.Adapt(v.VideoFull),
            VideoRaw = _adapter.Adapt(v.VideoRaw),
            Self = _urlSvc.GetVideoUrl(v.Id),
            CategoryLink = _urlSvc.GetCategoryUrl(v.CategoryId),
            CommentsLink = _urlSvc.GetCommentsUrl(v.Id),
            RatingLink = _urlSvc.GetRatingUrl(v.Id)
        };
    }

    public IEnumerable<MawApi.ViewModels.Videos.VideoViewModel> Adapt(IEnumerable<Video> videos)
    {
        return videos.Select(v => Adapt(v));
    }
}
