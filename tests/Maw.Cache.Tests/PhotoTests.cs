using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Maw.Domain.Models;
using Maw.Domain.Models.Photos;

namespace Maw.Cache.Tests;

public class PhotoTests
{
    [Fact]
    public async Task Categories_CacheMatchesData()
    {
        var dbCategories = await TestHelper.PhotoRepository.GetAllCategoriesAsync(TestHelper.Roles);
        var securedCategories = dbCategories.Select(x => new SecuredResource<Category>(x, TestHelper.Roles));

        await TestHelper.PhotoCache.AddCategoriesAsync(securedCategories);

        var cacheCategories = await TestHelper.PhotoCache.GetCategoriesAsync(TestHelper.Roles);

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
        Assert.Equal(dbFirst.PhotoCount, cacheFirst.PhotoCount);
        Assert.Equal(dbFirst.TeaserImage.Height, cacheFirst.TeaserImage.Height);
        Assert.Equal(dbFirst.TeaserImage.Width, cacheFirst.TeaserImage.Width);
        Assert.Equal(dbFirst.TeaserImage.Path, cacheFirst.TeaserImage.Path);
        Assert.Equal(dbFirst.TeaserImage.Size, cacheFirst.TeaserImage.Size);
        Assert.Equal(dbFirst.TeaserImageSq.Height, cacheFirst.TeaserImageSq.Height);
        Assert.Equal(dbFirst.TeaserImageSq.Width, cacheFirst.TeaserImageSq.Width);
        Assert.Equal(dbFirst.TeaserImageSq.Path, cacheFirst.TeaserImageSq.Path);
        Assert.Equal(dbFirst.TeaserImageSq.Size, cacheFirst.TeaserImageSq.Size);
        Assert.Equal(dbFirst.TotalSize, cacheFirst.TotalSize);
        Assert.Equal(dbFirst.TotalSizeLg, cacheFirst.TotalSizeLg);
        Assert.Equal(dbFirst.TotalSizeMd, cacheFirst.TotalSizeMd);
        Assert.Equal(dbFirst.TotalSizePrt, cacheFirst.TotalSizePrt);
        Assert.Equal(dbFirst.TotalSizeSm, cacheFirst.TotalSizeSm);
        Assert.Equal(dbFirst.TotalSizeSrc, cacheFirst.TotalSizeSrc);
        Assert.Equal(dbFirst.TotalSizeXs, cacheFirst.TotalSizeXs);
        Assert.Equal(dbFirst.TotalSizeXsSq, cacheFirst.TotalSizeXsSq);
        Assert.Equal(dbFirst.Year, cacheFirst.Year);

        // RECENT CATEGORIES
        var nextToLastId = (short) (dbCategories.Last().Id - 1);
        var dbRecent = await TestHelper.PhotoRepository.GetRecentCategoriesAsync(nextToLastId, TestHelper.Roles);
        var cacheRecent = await TestHelper.PhotoCache.GetRecentCategoriesAsync(TestHelper.Roles, nextToLastId);

        Assert.Equal(dbRecent.Count(), cacheRecent.Count());
        Assert.Equal(dbRecent.First().Id, cacheRecent.First().Id);

        // YEARS
        var dbYears = await TestHelper.PhotoRepository.GetYearsAsync(TestHelper.Roles);
        var cacheYears = await TestHelper.PhotoCache.GetYearsAsync(TestHelper.Roles);

        Assert.Equal(dbYears.Count(), cacheYears.Count());
        Assert.Equal(dbYears.First(), cacheYears.First());
    }

    [Fact]
    public async Task Photos_CacheMatchesData()
    {
        var dbPhotos = await TestHelper.PhotoRepository.GetPhotosForCategoryAsync(123, TestHelper.Roles);

        await TestHelper.PhotoCache.AddPhotosAsync(dbPhotos);

        var cachePhotos = await TestHelper.PhotoCache.GetPhotosAsync(TestHelper.Roles, 123);

        Assert.Equal(dbPhotos.Count(), cachePhotos.Count());

        var dbFirst = dbPhotos.First();
        var cacheFirst = cachePhotos.First();

        Assert.Equal(dbFirst.CategoryId, cacheFirst.CategoryId);
        Assert.Equal(dbFirst.CreateDate, cacheFirst.CreateDate);
        Assert.Equal(dbFirst.Id, cacheFirst.Id);
        Assert.Equal(dbFirst.Latitude, cacheFirst.Latitude);
        Assert.Equal(dbFirst.Longitude, cacheFirst.Longitude);
        Assert.Equal(dbFirst.LgInfo.Height, cacheFirst.LgInfo.Height);
        Assert.Equal(dbFirst.LgInfo.Width, cacheFirst.LgInfo.Width);
        Assert.Equal(dbFirst.LgInfo.Path, cacheFirst.LgInfo.Path);
        Assert.Equal(dbFirst.LgInfo.Size, cacheFirst.LgInfo.Size);
        Assert.Equal(dbFirst.MdInfo.Height, cacheFirst.MdInfo.Height);
        Assert.Equal(dbFirst.MdInfo.Width, cacheFirst.MdInfo.Width);
        Assert.Equal(dbFirst.MdInfo.Path, cacheFirst.MdInfo.Path);
        Assert.Equal(dbFirst.MdInfo.Size, cacheFirst.MdInfo.Size);
        Assert.Equal(dbFirst.PrtInfo.Height, cacheFirst.PrtInfo.Height);
        Assert.Equal(dbFirst.PrtInfo.Width, cacheFirst.PrtInfo.Width);
        Assert.Equal(dbFirst.PrtInfo.Path, cacheFirst.PrtInfo.Path);
        Assert.Equal(dbFirst.PrtInfo.Size, cacheFirst.PrtInfo.Size);
        Assert.Equal(dbFirst.SmInfo.Height, cacheFirst.SmInfo.Height);
        Assert.Equal(dbFirst.SmInfo.Width, cacheFirst.SmInfo.Width);
        Assert.Equal(dbFirst.SmInfo.Path, cacheFirst.SmInfo.Path);
        Assert.Equal(dbFirst.SmInfo.Size, cacheFirst.SmInfo.Size);
        Assert.Equal(dbFirst.SrcInfo.Height, cacheFirst.SrcInfo.Height);
        Assert.Equal(dbFirst.SrcInfo.Width, cacheFirst.SrcInfo.Width);
        Assert.Equal(dbFirst.SrcInfo.Path, cacheFirst.SrcInfo.Path);
        Assert.Equal(dbFirst.SrcInfo.Size, cacheFirst.SrcInfo.Size);
        Assert.Equal(dbFirst.XsInfo.Height, cacheFirst.XsInfo.Height);
        Assert.Equal(dbFirst.XsInfo.Width, cacheFirst.XsInfo.Width);
        Assert.Equal(dbFirst.XsInfo.Path, cacheFirst.XsInfo.Path);
        Assert.Equal(dbFirst.XsInfo.Size, cacheFirst.XsInfo.Size);
        Assert.Equal(dbFirst.XsSqInfo.Height, cacheFirst.XsSqInfo.Height);
        Assert.Equal(dbFirst.XsSqInfo.Width, cacheFirst.XsSqInfo.Width);
        Assert.Equal(dbFirst.XsSqInfo.Path, cacheFirst.XsSqInfo.Path);
        Assert.Equal(dbFirst.XsSqInfo.Size, cacheFirst.XsSqInfo.Size);

        var cacheRandomPhotos = await TestHelper.PhotoCache.GetRandomPhotosAsync(TestHelper.Roles, 2);

        Assert.Equal(2, cacheRandomPhotos.Count());
    }
}
