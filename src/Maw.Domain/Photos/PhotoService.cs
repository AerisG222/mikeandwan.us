﻿using Microsoft.Extensions.Logging;
using Maw.Cache.Abstractions;
using Maw.Data.Abstractions;
using Maw.Domain.Models;
using Maw.Domain.Models.Photos;

namespace Maw.Domain.Photos;

public class PhotoService
    : BaseService, IPhotoService
{
    readonly IPhotoRepository _repo;
    readonly IPhotoCache _cache;

    public PhotoService(
        IPhotoRepository photoRepository,
        IPhotoCache cache,
        ILogger<PhotoService> log)
        : base(log)
    {
        _repo = photoRepository ?? throw new ArgumentNullException(nameof(photoRepository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public Task<Photo?> GetRandomAsync(string[] roles)
    {
        return _repo.GetRandomAsync(roles);
    }

    public async Task<IEnumerable<Photo>> GetRandomAsync(byte count, string[] roles)
    {
        var photos = await _repo.GetRandomAsync(count, roles);

        return photos ?? new List<Photo>();
    }

    public async Task<IEnumerable<short>> GetYearsAsync(string[] roles)
    {
        var years = await GetCachedValueAsync(
            () => _cache.GetYearsAsync(roles),
            () => _repo.GetYearsAsync(roles)
        );

        return years ?? new List<short>();
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(string[] roles)
    {
        var categories = await GetCachedValueAsync(
            () => _cache.GetCategoriesAsync(roles),
            () => _repo.GetAllCategoriesAsync(roles)
        );

        return categories ?? new List<Category>();
    }

    public async Task<IEnumerable<Category>> GetCategoriesForYearAsync(short year, string[] roles)
    {
        var categories = await GetCachedValueAsync(
            () => _cache.GetCategoriesAsync(roles, year),
            () => _repo.GetCategoriesForYearAsync(year, roles)
        );

        return categories ?? new List<Category>();
    }

    public async Task<IEnumerable<Category>> GetRecentCategoriesAsync(short sinceId, string[] roles)
    {
        return await GetCachedValueAsync(
            () => _cache.GetRecentCategoriesAsync(roles, sinceId),
            () => _repo.GetRecentCategoriesAsync(sinceId, roles)
        ) ;
    }

    public async Task<IEnumerable<Photo>> GetPhotosForCategoryAsync(short categoryId, string[] roles)
    {
        var photos = await GetCachedValueAsync(
            () => _cache.GetPhotosAsync(roles, categoryId),
            () => _repo.GetPhotosForCategoryAsync(categoryId, roles)
        );

        return photos ?? new List<Photo>();
    }

    public Task<Category?> GetCategoryAsync(short categoryId, string[] roles)
    {
        return GetNullableCachedValueAsync(
            () => _cache.GetCategoryAsync(roles, categoryId),
            () => _repo.GetCategoryAsync(categoryId, roles)
        );
    }

    public Task<Photo?> GetPhotoAsync(int photoId, string[] roles)
    {
        return GetNullableCachedValueAsync(
            () => _cache.GetPhotoAsync(roles, photoId),
            () => _repo.GetPhotoAsync(photoId, roles)
        );
    }

    public async Task<Detail?> GetDetailAsync(int photoId, string[] roles)
    {
        return await GetOrSetCachedValueAsync(
            () => _cache.GetPhotoDetailsAsync(roles, photoId),
            () => _repo.GetDetailAsync(photoId, roles),
            detail => _cache.AddPhotoDetailsAsync(photoId, detail)
        );
    }

    public Task<IEnumerable<Comment>> GetCommentsAsync(int photoId, string[] roles)
    {
        return _repo.GetCommentsAsync(photoId, roles);
    }

    public Task<Rating?> GetRatingsAsync(int photoId, string username, string[] roles)
    {
        return _repo.GetRatingsAsync(photoId, username, roles);
    }

    public Task<GpsDetail?> GetGpsDetailAsync(int photoId, string[] roles)
    {
        return _repo.GetGpsDetailAsync(photoId, roles);
    }

    public Task InsertCommentAsync(int photoId, string username, string comment, string[] roles)
    {
        return _repo.InsertCommentAsync(photoId, username, comment, roles);
    }

    public Task<float?> SaveRatingAsync(int photoId, string username, short rating, string[] roles)
    {
        return _repo.SaveRatingAsync(photoId, username, rating, roles);
    }

    public Task<float?> RemoveRatingAsync(int photoId, string username, string[] roles)
    {
        return _repo.RemoveRatingAsync(photoId, username, roles);
    }

    public Task SetGpsOverrideAsync(int photoId, GpsCoordinate gps, string username)
    {
        return _repo.SetGpsOverrideAsync(photoId, gps, username);
    }

    public async Task SetCategoryTeaserAsync(short categoryId, int photoId)
    {
        var count = await _repo.SetCategoryTeaserAsync(categoryId, photoId);

        if (count != 1)
        {
            throw new ApplicationException("Did not update category teaser!");
        }

        var cat = await _repo.GetCategoryAsync(categoryId, null);

        if(cat != null)
        {
            await _cache.AddCategoryAsync(new SecuredResource<Category>(cat, Array.Empty<string>()));
        }
    }

    public Task<IEnumerable<CategoryAndRoles>> GetCategoriesAndRolesAsync()
    {
        return _repo.GetCategoriesAndRolesAsync();
    }

    public Task ClearCacheAsync()
    {
        return Task.CompletedTask;
    }
}
