using Maw.Domain.Models;
using MawApi.Services.Photos;
using MawApi.ViewModels;

namespace MawApi.Services;

public class PhotoMultimediaInfoAdapter
{
    readonly PhotoUrlBuilderService _urlSvc;

    public PhotoMultimediaInfoAdapter(PhotoUrlBuilderService urlSvc)
    {
        ArgumentNullException.ThrowIfNull(urlSvc);

        _urlSvc = urlSvc;
    }

    public PhotoMultimediaAsset Adapt(MultimediaInfo info, int photoId, string size)
    {
        if(info == null)
        {
            return new PhotoMultimediaAsset();
        }

        return new PhotoMultimediaAsset {
            Height = info.Height,
            Width = info.Width,
            Size = info.Size,
            Url = _urlSvc.GetImageUrl(info.Path),
            DownloadUrl = _urlSvc.GetImageDownloadUrl(photoId, size)
        };
    }
}
