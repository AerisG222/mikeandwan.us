using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Maw.Domain.Models;
using Maw.Domain.Models.Videos;
using Maw.Cache.Abstractions;

namespace Maw.Cache.Tests;

public class VideoTests
{
    [Fact]
    public async Task Categories_CacheMatchesData()
    {
        var dbCategories = await TestHelper.VideoRepository.GetAllCategoriesAsync(TestHelper.Roles);
        var securedCategories = dbCategories.Select(x => new SecuredResource<Category>(x, TestHelper.Roles));

        await TestHelper.VideoCache.AddCategoriesAsync(securedCategories);
        await TestHelper.VideoCache.SetStatusAsync(CacheStatus.InitializationSucceeded);

        var cacheCategories = (await TestHelper.VideoCache.GetCategoriesAsync(TestHelper.Roles)).Value;

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
        var cacheYears = (await TestHelper.VideoCache.GetYearsAsync(TestHelper.Roles)).Value;

        Assert.Equal(dbYears.Count(), cacheYears.Count());
        Assert.Equal(dbYears.First(), cacheYears.First());
    }

    [Fact]
    public async Task Videos_CacheMatchesData()
    {
        var dbVideos = await TestHelper.VideoRepository.GetVideosInCategoryAsync(123, TestHelper.Roles);
        var securedVideos = dbVideos.Select(x => new SecuredResource<Video>(x, TestHelper.Roles));

        await TestHelper.VideoCache.AddVideosAsync(securedVideos);
        await TestHelper.VideoCache.SetStatusAsync(CacheStatus.InitializationSucceeded);

        var cacheVideos = (await TestHelper.VideoCache.GetVideosAsync(TestHelper.Roles, 123)).Value;

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

        var singleCachedVideo = (await TestHelper.VideoCache.GetVideoAsync(TestHelper.Roles, cacheFirst.Id)).Value;

        Assert.NotNull(singleCachedVideo);
        Assert.Equal(cacheFirst.CategoryId, singleCachedVideo!.CategoryId);
        Assert.Equal(cacheFirst.CreateDate, singleCachedVideo!.CreateDate);
        Assert.Equal(cacheFirst.Duration, singleCachedVideo!.Duration);
        Assert.Equal(cacheFirst.Id, singleCachedVideo!.Id);
        Assert.Equal(cacheFirst.Latitude, singleCachedVideo!.Latitude);
        Assert.Equal(cacheFirst.Longitude, singleCachedVideo!.Longitude);
        Assert.Equal(cacheFirst.Thumbnail.Height, singleCachedVideo!.Thumbnail.Height);
        Assert.Equal(cacheFirst.Thumbnail.Width, singleCachedVideo!.Thumbnail.Width);
        Assert.Equal(cacheFirst.Thumbnail.Path, singleCachedVideo!.Thumbnail.Path);
        Assert.Equal(cacheFirst.Thumbnail.Size, singleCachedVideo!.Thumbnail.Size);
        Assert.Equal(cacheFirst.ThumbnailSq.Height, singleCachedVideo!.ThumbnailSq.Height);
        Assert.Equal(cacheFirst.ThumbnailSq.Width, singleCachedVideo!.ThumbnailSq.Width);
        Assert.Equal(cacheFirst.ThumbnailSq.Path, singleCachedVideo!.ThumbnailSq.Path);
        Assert.Equal(cacheFirst.ThumbnailSq.Size, singleCachedVideo!.ThumbnailSq.Size);
        Assert.Equal(cacheFirst.VideoFull.Height, singleCachedVideo!.VideoFull.Height);
        Assert.Equal(cacheFirst.VideoFull.Width, singleCachedVideo!.VideoFull.Width);
        Assert.Equal(cacheFirst.VideoFull.Path, singleCachedVideo!.VideoFull.Path);
        Assert.Equal(cacheFirst.VideoFull.Size, singleCachedVideo!.VideoFull.Size);
        Assert.Equal(cacheFirst.VideoRaw.Height, singleCachedVideo!.VideoRaw.Height);
        Assert.Equal(cacheFirst.VideoRaw.Width, singleCachedVideo!.VideoRaw.Width);
        Assert.Equal(cacheFirst.VideoRaw.Path, singleCachedVideo!.VideoRaw.Path);
        Assert.Equal(cacheFirst.VideoRaw.Size, singleCachedVideo!.VideoRaw.Size);
        Assert.Equal(cacheFirst.VideoScaled.Height, singleCachedVideo!.VideoScaled.Height);
        Assert.Equal(cacheFirst.VideoScaled.Width, singleCachedVideo!.VideoScaled.Width);
        Assert.Equal(cacheFirst.VideoScaled.Path, singleCachedVideo!.VideoScaled.Path);
        Assert.Equal(cacheFirst.VideoScaled.Size, singleCachedVideo!.VideoScaled.Size);
    }
}
