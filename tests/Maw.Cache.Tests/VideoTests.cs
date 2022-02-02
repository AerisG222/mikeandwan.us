using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Maw.Domain.Models;
using Maw.Domain.Models.Videos;

namespace Maw.Cache.Tests;

public class VideoTests
{
    [Fact]
    public async Task Categories_CacheMatchesData()
    {
        var dbCategories = await TestHelper.VideoRepository.GetAllCategoriesAsync(TestHelper.Roles);
        var securedCategories = dbCategories.Select(x => new SecuredResource<Category>(x, TestHelper.Roles));

        await TestHelper.VideoCache.AddCategoriesAsync(securedCategories);

        var cacheCategories = await TestHelper.VideoCache.GetCategoriesAsync(TestHelper.Roles);

        Assert.Equal(dbCategories.Count(), cacheCategories.Count());

        // CATEGORY DETAILS
        var dbFirst = dbCategories.First();
        var cacheFirst = cacheCategories.First();

        Assert.Equal(dbFirst.CreateDate, cacheFirst.CreateDate);
        Assert.Equal(dbFirst.Id, cacheFirst.Id);
        Assert.Equal(dbFirst.IsMissingGpsData, cacheFirst.IsMissingGpsData);
        Assert.Equal(dbFirst.Latitude, cacheFirst.Latitude);
        Assert.Equal(dbFirst.Longitude, cacheFirst.Longitude);
        Assert.Equal(dbFirst.Name, cacheFirst.Name);
        Assert.Equal(dbFirst.VideoCount, cacheFirst.VideoCount);
        Assert.Equal(dbFirst.TeaserImage.Height, cacheFirst.TeaserImage.Height);
        Assert.Equal(dbFirst.TeaserImage.Width, cacheFirst.TeaserImage.Width);
        Assert.Equal(dbFirst.TeaserImage.Path, cacheFirst.TeaserImage.Path);
        Assert.Equal(dbFirst.TeaserImage.Size, cacheFirst.TeaserImage.Size);
        Assert.Equal(dbFirst.TeaserImageSq.Height, cacheFirst.TeaserImageSq.Height);
        Assert.Equal(dbFirst.TeaserImageSq.Width, cacheFirst.TeaserImageSq.Width);
        Assert.Equal(dbFirst.TeaserImageSq.Path, cacheFirst.TeaserImageSq.Path);
        Assert.Equal(dbFirst.TeaserImageSq.Size, cacheFirst.TeaserImageSq.Size);
        Assert.Equal(dbFirst.TotalDuration, cacheFirst.TotalDuration);
        Assert.Equal(dbFirst.TotalSize, cacheFirst.TotalSize);
        Assert.Equal(dbFirst.TotalSizeFull, cacheFirst.TotalSizeFull);
        Assert.Equal(dbFirst.TotalSizeRaw, cacheFirst.TotalSizeRaw);
        Assert.Equal(dbFirst.TotalSizeScaled, cacheFirst.TotalSizeScaled);
        Assert.Equal(dbFirst.TotalSizeThumbnail, cacheFirst.TotalSizeThumbnail);
        Assert.Equal(dbFirst.TotalSizeThumbnailSq, cacheFirst.TotalSizeThumbnailSq);
        Assert.Equal(dbFirst.Year, cacheFirst.Year);

        // YEARS
        var dbYears = await TestHelper.VideoRepository.GetYearsAsync(TestHelper.Roles);
        var cacheYears = await TestHelper.VideoCache.GetYearsAsync(TestHelper.Roles);

        Assert.Equal(dbYears.Count(), cacheYears.Count());
        Assert.Equal(dbYears.First(), cacheYears.First());
    }

    [Fact]
    public async Task Videos_CacheMatchesData()
    {
        var dbVideos = await TestHelper.VideoRepository.GetVideosInCategoryAsync(123, TestHelper.Roles);

        await TestHelper.VideoCache.AddVideosAsync(dbVideos);

        var cacheVideos = await TestHelper.VideoCache.GetVideosAsync(TestHelper.Roles, 123);

        Assert.Equal(dbVideos.Count(), cacheVideos.Count());

        var dbFirst = dbVideos.First();
        var cacheFirst = cacheVideos.First();

        Assert.Equal(dbFirst.CategoryId, cacheFirst.CategoryId);
        Assert.Equal(dbFirst.CreateDate, cacheFirst.CreateDate);
        Assert.Equal(dbFirst.Duration, cacheFirst.Duration);
        Assert.Equal(dbFirst.Id, cacheFirst.Id);
        Assert.Equal(dbFirst.Latitude, cacheFirst.Latitude);
        Assert.Equal(dbFirst.Longitude, cacheFirst.Longitude);
        Assert.Equal(dbFirst.Thumbnail.Height, cacheFirst.Thumbnail.Height);
        Assert.Equal(dbFirst.Thumbnail.Width, cacheFirst.Thumbnail.Width);
        Assert.Equal(dbFirst.Thumbnail.Path, cacheFirst.Thumbnail.Path);
        Assert.Equal(dbFirst.Thumbnail.Size, cacheFirst.Thumbnail.Size);
        Assert.Equal(dbFirst.ThumbnailSq.Height, cacheFirst.ThumbnailSq.Height);
        Assert.Equal(dbFirst.ThumbnailSq.Width, cacheFirst.ThumbnailSq.Width);
        Assert.Equal(dbFirst.ThumbnailSq.Path, cacheFirst.ThumbnailSq.Path);
        Assert.Equal(dbFirst.ThumbnailSq.Size, cacheFirst.ThumbnailSq.Size);
        Assert.Equal(dbFirst.VideoFull.Height, cacheFirst.VideoFull.Height);
        Assert.Equal(dbFirst.VideoFull.Width, cacheFirst.VideoFull.Width);
        Assert.Equal(dbFirst.VideoFull.Path, cacheFirst.VideoFull.Path);
        Assert.Equal(dbFirst.VideoFull.Size, cacheFirst.VideoFull.Size);
        Assert.Equal(dbFirst.VideoRaw.Height, cacheFirst.VideoRaw.Height);
        Assert.Equal(dbFirst.VideoRaw.Width, cacheFirst.VideoRaw.Width);
        Assert.Equal(dbFirst.VideoRaw.Path, cacheFirst.VideoRaw.Path);
        Assert.Equal(dbFirst.VideoRaw.Size, cacheFirst.VideoRaw.Size);
        Assert.Equal(dbFirst.VideoScaled.Height, cacheFirst.VideoScaled.Height);
        Assert.Equal(dbFirst.VideoScaled.Width, cacheFirst.VideoScaled.Width);
        Assert.Equal(dbFirst.VideoScaled.Path, cacheFirst.VideoScaled.Path);
        Assert.Equal(dbFirst.VideoScaled.Size, cacheFirst.VideoScaled.Size);
    }
}
