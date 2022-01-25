using Maw.Domain.Models;
using MawApi.Services.Photos;
using MawApi.ViewModels;

namespace MawApi.Services;

public class MultimediaInfoAdapter
{
    readonly PhotoUrlBuilderService _urlSvc;

    public MultimediaInfoAdapter(PhotoUrlBuilderService urlSvc)
    {
        _urlSvc = urlSvc ?? throw new ArgumentNullException(nameof(urlSvc));
    }

    public MultimediaAsset Adapt(MultimediaInfo info)
    {
        if(info == null)
        {
            return new MultimediaAsset();
        }

        return new MultimediaAsset {
            Height = info.Height,
            Width = info.Width,
            Size = info.Size,
            Url = _urlSvc.GetImageUrl(info.Path)
        };
    }
}
