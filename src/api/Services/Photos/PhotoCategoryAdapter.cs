using Maw.Domain.Models.Photos;
using MawApi.ViewModels.Photos;

namespace MawApi.Services.Photos;

public class PhotoCategoryAdapter
{
    readonly PhotoUrlBuilderService _urlSvc;
    readonly MultimediaInfoAdapter _adapter;

    public PhotoCategoryAdapter(
        PhotoUrlBuilderService urlSvc,
        MultimediaInfoAdapter adapter)
    {
        ArgumentNullException.ThrowIfNull(urlSvc);
        ArgumentNullException.ThrowIfNull(adapter);

        _urlSvc = urlSvc;
        _adapter = adapter;
    }

    public PhotoCategoryViewModel Adapt(Category c)
    {
        ArgumentNullException.ThrowIfNull(c);

        return new PhotoCategoryViewModel {
            Id = c.Id,
            Name = c.Name,
            Year = c.Year,
            CreateDate = c.CreateDate,
            Latitude = c.Latitude,
            Longitude = c.Longitude,
            PhotoCount = c.PhotoCount,
            TotalSizeXs = c.TotalSizeXs,
            TotalSizeXsSq = c.TotalSizeXsSq,
            TotalSizeSm = c.TotalSizeSm,
            TotalSizeMd = c.TotalSizeMd,
            TotalSizeLg = c.TotalSizeLg,
            TotalSizePrt = c.TotalSizePrt,
            TotalSizeSrc = c.TotalSizeSrc,
            TotalSize = c.TotalSize,
            TeaserImage = _adapter.Adapt(c.TeaserImage),
            TeaserImageSq = _adapter.Adapt(c.TeaserImageSq),
            Self = _urlSvc.GetCategoryUrl(c.Id),
            PhotosLink = _urlSvc.GetPhotosUrl(c.Id),
            DownloadLink = _urlSvc.GetCategoryDownloadUrl(c.Id),
            IsMissingGpsData = c.IsMissingGpsData
        };
    }

    public IEnumerable<MawApi.ViewModels.Photos.PhotoCategoryViewModel> Adapt(IEnumerable<Category> categories)
    {
        return categories.Select(c => Adapt(c));
    }
}
