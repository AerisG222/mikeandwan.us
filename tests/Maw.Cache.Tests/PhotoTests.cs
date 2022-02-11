using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Maw.Domain.Models;
using Maw.Domain.Models.Photos;
using Maw.Cache.Abstractions;

namespace Maw.Cache.Tests;

public class PhotoTests
{
    [Fact]
    public async Task Categories_CacheMatchesData()
    {
        var dbCategories = await TestHelper.PhotoRepository.GetAllCategoriesAsync(TestHelper.Roles);
        var securedCategories = dbCategories.Select(x => new SecuredResource<Category>(x, TestHelper.Roles));

        await TestHelper.PhotoCache.AddCategoriesAsync(securedCategories);
        await TestHelper.PhotoCache.SetStatusAsync(CacheStatus.InitializationSucceeded);

        var cacheCategories = (await TestHelper.PhotoCache.GetCategoriesAsync(TestHelper.Roles)).Value;

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
        var cacheRecent = (await TestHelper.PhotoCache.GetRecentCategoriesAsync(TestHelper.Roles, nextToLastId)).Value;

        Assert.Equal(dbRecent.Count(), cacheRecent.Count());
        Assert.Equal(dbRecent.First().Id, cacheRecent.First().Id);

        // YEARS
        var dbYears = await TestHelper.PhotoRepository.GetYearsAsync(TestHelper.Roles);
        var cacheYears = (await TestHelper.PhotoCache.GetYearsAsync(TestHelper.Roles)).Value;

        Assert.Equal(dbYears.Count(), cacheYears.Count());
        Assert.Equal(dbYears.First(), cacheYears.First());
    }

    [Fact]
    public async Task Photos_CacheMatchesData()
    {
        var dbPhotos = await TestHelper.PhotoRepository.GetPhotosForCategoryAsync(123, TestHelper.Roles);
        var securedPhotos = dbPhotos.Select(x => new SecuredResource<Photo>(x, TestHelper.Roles));

        await TestHelper.PhotoCache.AddPhotosAsync(securedPhotos);
        await TestHelper.PhotoCache.SetStatusAsync(CacheStatus.InitializationSucceeded);

        var cachePhotos = (await TestHelper.PhotoCache.GetPhotosAsync(TestHelper.Roles, 123)).Value;

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

        var cacheRandomPhotos = (await TestHelper.PhotoCache.GetRandomPhotosAsync(TestHelper.Roles, 2)).Value;

        Assert.Equal(2, cacheRandomPhotos.Count());

        var singleCachedPhoto = (await TestHelper.PhotoCache.GetPhotoAsync(TestHelper.Roles, cacheFirst.Id)).Value;

        Assert.NotNull(singleCachedPhoto);
        Assert.Equal(cacheFirst.Id, singleCachedPhoto!.Id);
        Assert.Equal(cacheFirst.CategoryId, singleCachedPhoto!.CategoryId);
        Assert.Equal(cacheFirst.CreateDate, singleCachedPhoto!.CreateDate);
        Assert.Equal(cacheFirst.Id, singleCachedPhoto!.Id);
        Assert.Equal(cacheFirst.Latitude, singleCachedPhoto!.Latitude);
        Assert.Equal(cacheFirst.Longitude, singleCachedPhoto!.Longitude);
        Assert.Equal(cacheFirst.LgInfo.Height, singleCachedPhoto!.LgInfo.Height);
        Assert.Equal(cacheFirst.LgInfo.Width, singleCachedPhoto!.LgInfo.Width);
        Assert.Equal(cacheFirst.LgInfo.Path, singleCachedPhoto!.LgInfo.Path);
        Assert.Equal(cacheFirst.LgInfo.Size, singleCachedPhoto!.LgInfo.Size);
        Assert.Equal(cacheFirst.MdInfo.Height, singleCachedPhoto!.MdInfo.Height);
        Assert.Equal(cacheFirst.MdInfo.Width, singleCachedPhoto!.MdInfo.Width);
        Assert.Equal(cacheFirst.MdInfo.Path, singleCachedPhoto!.MdInfo.Path);
        Assert.Equal(cacheFirst.MdInfo.Size, singleCachedPhoto!.MdInfo.Size);
        Assert.Equal(cacheFirst.PrtInfo.Height, singleCachedPhoto!.PrtInfo.Height);
        Assert.Equal(cacheFirst.PrtInfo.Width, singleCachedPhoto!.PrtInfo.Width);
        Assert.Equal(cacheFirst.PrtInfo.Path, singleCachedPhoto!.PrtInfo.Path);
        Assert.Equal(cacheFirst.PrtInfo.Size, singleCachedPhoto!.PrtInfo.Size);
        Assert.Equal(cacheFirst.SmInfo.Height, singleCachedPhoto!.SmInfo.Height);
        Assert.Equal(cacheFirst.SmInfo.Width, singleCachedPhoto!.SmInfo.Width);
        Assert.Equal(cacheFirst.SmInfo.Path, singleCachedPhoto!.SmInfo.Path);
        Assert.Equal(cacheFirst.SmInfo.Size, singleCachedPhoto!.SmInfo.Size);
        Assert.Equal(cacheFirst.SrcInfo.Height, singleCachedPhoto!.SrcInfo.Height);
        Assert.Equal(cacheFirst.SrcInfo.Width, singleCachedPhoto!.SrcInfo.Width);
        Assert.Equal(cacheFirst.SrcInfo.Path, singleCachedPhoto!.SrcInfo.Path);
        Assert.Equal(cacheFirst.SrcInfo.Size, singleCachedPhoto!.SrcInfo.Size);
        Assert.Equal(cacheFirst.XsInfo.Height, singleCachedPhoto!.XsInfo.Height);
        Assert.Equal(cacheFirst.XsInfo.Width, singleCachedPhoto!.XsInfo.Width);
        Assert.Equal(cacheFirst.XsInfo.Path, singleCachedPhoto!.XsInfo.Path);
        Assert.Equal(cacheFirst.XsInfo.Size, singleCachedPhoto!.XsInfo.Size);
        Assert.Equal(cacheFirst.XsSqInfo.Height, singleCachedPhoto!.XsSqInfo.Height);
        Assert.Equal(cacheFirst.XsSqInfo.Width, singleCachedPhoto!.XsSqInfo.Width);
        Assert.Equal(cacheFirst.XsSqInfo.Path, singleCachedPhoto!.XsSqInfo.Path);
        Assert.Equal(cacheFirst.XsSqInfo.Size, singleCachedPhoto!.XsSqInfo.Size);

        var dbDetail = await TestHelper.PhotoRepository.GetDetailAsync(cacheFirst.Id, TestHelper.Roles);

        await TestHelper.PhotoCache.AddPhotoDetailsAsync(cacheFirst.Id, dbDetail!);

        var cacheDetail = await TestHelper.PhotoCache.GetPhotoDetailsAsync(TestHelper.Roles, cacheFirst.Id);

        Assert.NotNull(cacheDetail);
        Assert.Equal(dbDetail!.BitsPerSample, cacheDetail!.BitsPerSample);
        Assert.Equal(dbDetail!.Compression, cacheDetail!.Compression);
        Assert.Equal(dbDetail!.Contrast, cacheDetail!.Contrast);
        Assert.Equal(dbDetail!.CreateDate, cacheDetail!.CreateDate);
        Assert.Equal(dbDetail!.DigitalZoomRatio, cacheDetail!.DigitalZoomRatio);
        Assert.Equal(dbDetail!.ExposureCompensation, cacheDetail!.ExposureCompensation);
        Assert.Equal(dbDetail!.ExposureMode, cacheDetail!.ExposureMode);
        Assert.Equal(dbDetail!.ExposureProgram, cacheDetail!.ExposureProgram);
        Assert.Equal(dbDetail!.ExposureTime, cacheDetail!.ExposureTime);
        Assert.Equal(dbDetail!.FNumber, cacheDetail!.FNumber);
        Assert.Equal(dbDetail!.Flash, cacheDetail!.Flash);
        Assert.Equal(dbDetail!.FocalLength, cacheDetail!.FocalLength);
        Assert.Equal(dbDetail!.FocalLengthIn35mmFormat, cacheDetail!.FocalLengthIn35mmFormat);
        Assert.Equal(dbDetail!.GainControl, cacheDetail!.GainControl);
        Assert.Equal(dbDetail!.GpsAltitude, cacheDetail!.GpsAltitude);
        Assert.Equal(dbDetail!.GpsAltitudeRef, cacheDetail!.GpsAltitudeRef);
        Assert.Equal(dbDetail!.GpsDateStamp, cacheDetail!.GpsDateStamp);
        Assert.Equal(dbDetail!.GpsDirection, cacheDetail!.GpsDirection);
        Assert.Equal(dbDetail!.GpsDirectionRef, cacheDetail!.GpsDirectionRef);
        Assert.Equal(dbDetail!.GpsLatitude, cacheDetail!.GpsLatitude);
        Assert.Equal(dbDetail!.GpsLatitudeRef, cacheDetail!.GpsLatitudeRef);
        Assert.Equal(dbDetail!.GpsLongitude, cacheDetail!.GpsLongitude);
        Assert.Equal(dbDetail!.GpsLongitudeRef, cacheDetail!.GpsLongitudeRef);
        Assert.Equal(dbDetail!.GpsMeasureMode, cacheDetail!.GpsMeasureMode);
        Assert.Equal(dbDetail!.GpsSatellites, cacheDetail!.GpsSatellites);
        Assert.Equal(dbDetail!.GpsStatus, cacheDetail!.GpsStatus);
        Assert.Equal(dbDetail!.GpsVersionId, cacheDetail!.GpsVersionId);
        Assert.Equal(dbDetail!.Iso, cacheDetail!.Iso);
        Assert.Equal(dbDetail!.LightSource, cacheDetail!.LightSource);
        Assert.Equal(dbDetail!.Make, cacheDetail!.Make);
        Assert.Equal(dbDetail!.MeteringMode, cacheDetail!.MeteringMode);
        Assert.Equal(dbDetail!.Model, cacheDetail!.Model);
        Assert.Equal(dbDetail!.Orientation, cacheDetail!.Orientation);
        Assert.Equal(dbDetail!.Saturation, cacheDetail!.Saturation);
        Assert.Equal(dbDetail!.SceneCaptureType, cacheDetail!.SceneCaptureType);
        Assert.Equal(dbDetail!.SceneType, cacheDetail!.SceneType);
        Assert.Equal(dbDetail!.SensingMethod, cacheDetail!.SensingMethod);
        Assert.Equal(dbDetail!.Sharpness, cacheDetail!.Sharpness);

        Assert.Equal(dbDetail!.AutoFocusAreaMode, cacheDetail!.AutoFocusAreaMode);
        Assert.Equal(dbDetail!.AutoFocusPoint, cacheDetail!.AutoFocusPoint);
        Assert.Equal(dbDetail!.ActiveDLighting, cacheDetail!.ActiveDLighting);
        Assert.Equal(dbDetail!.Colorspace, cacheDetail!.Colorspace);
        Assert.Equal(dbDetail!.ExposureDifference, cacheDetail!.ExposureDifference);
        Assert.Equal(dbDetail!.FlashColorFilter, cacheDetail!.FlashColorFilter);
        Assert.Equal(dbDetail!.FlashCompensation, cacheDetail!.FlashCompensation);
        Assert.Equal(dbDetail!.FlashControlMode, cacheDetail!.FlashControlMode);
        Assert.Equal(dbDetail!.FlashExposureCompensation, cacheDetail!.FlashExposureCompensation);
        Assert.Equal(dbDetail!.FlashFocalLength, cacheDetail!.FlashFocalLength);
        Assert.Equal(dbDetail!.FlashMode, cacheDetail!.FlashMode);
        Assert.Equal(dbDetail!.FlashSetting, cacheDetail!.FlashSetting);
        Assert.Equal(dbDetail!.FlashType, cacheDetail!.FlashType);
        Assert.Equal(dbDetail!.FocusDistance, cacheDetail!.FocusDistance);
        Assert.Equal(dbDetail!.FocusMode, cacheDetail!.FocusMode);
        Assert.Equal(dbDetail!.FocusPosition, cacheDetail!.FocusPosition);
        Assert.Equal(dbDetail!.HighIsoNoiseReduction, cacheDetail!.HighIsoNoiseReduction);
        Assert.Equal(dbDetail!.HueAdjustment, cacheDetail!.HueAdjustment);
        Assert.Equal(dbDetail!.NoiseReduction, cacheDetail!.NoiseReduction);
        Assert.Equal(dbDetail!.PictureControlName, cacheDetail!.PictureControlName);
        Assert.Equal(dbDetail!.PrimaryAFPoint, cacheDetail!.PrimaryAFPoint);
        Assert.Equal(dbDetail!.VRMode, cacheDetail!.VRMode);
        Assert.Equal(dbDetail!.VibrationReduction, cacheDetail!.VibrationReduction);
        Assert.Equal(dbDetail!.VignetteControl, cacheDetail!.VignetteControl);
        Assert.Equal(dbDetail!.WhiteBalance, cacheDetail!.WhiteBalance);

        Assert.Equal(dbDetail!.Aperture, cacheDetail!.Aperture);
        Assert.Equal(dbDetail!.AutoFocus, cacheDetail!.AutoFocus);
        Assert.Equal(dbDetail!.DepthOfField, cacheDetail!.DepthOfField);
        Assert.Equal(dbDetail!.FieldOfView, cacheDetail!.FieldOfView);
        Assert.Equal(dbDetail!.HyperfocalDistance, cacheDetail!.HyperfocalDistance);
        Assert.Equal(dbDetail!.LensId, cacheDetail!.LensId);
        Assert.Equal(dbDetail!.LightValue, cacheDetail!.LightValue);
        Assert.Equal(dbDetail!.ScaleFactor35Efl, cacheDetail!.ScaleFactor35Efl);
        Assert.Equal(dbDetail!.ShutterSpeed, cacheDetail!.ShutterSpeed);
    }
}
