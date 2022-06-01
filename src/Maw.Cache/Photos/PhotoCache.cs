using StackExchange.Redis;
using Maw.Cache.Abstractions;
using Maw.Domain.Models;
using Maw.Domain.Models.Photos;

namespace Maw.Cache.Photos;

public class PhotoCache
    : BaseCache, IPhotoCache
{
    readonly CategorySerializer _categorySerializer = new();
    readonly PhotoDetailSerializer _detailSerializer = new();
    readonly PhotoSerializer _photoSerializer = new();

    public PhotoCache(IDatabase redisDatabase)
        : base(redisDatabase, PhotoKeys.CACHE_STATUS)
    {

    }

    public async Task<CacheResult<IEnumerable<short>>> GetYearsAsync(string[] roles)
    {
        var tran = Db.CreateTransaction();
        var status = GetStatusAsync(tran);
        var accessibleCategoriesSetKey = PrepareAccessibleCategoriesSet(tran, roles);
        var years = tran.SortAsync(
            accessibleCategoriesSetKey,
            get: CategorySerializer.YearLookupField
        );

        await tran.ExecuteAsync();

        return BuildResult(
            await status,
            (await years)
                .Select(year => (short)year)
                .Distinct()
                .OrderByDescending(year => year)
                .AsEnumerable()
        );
    }

    public async Task<CacheResult<IEnumerable<Category>>> GetCategoriesAsync(string[] roles)
    {
        var tran = Db.CreateTransaction();
        var accessibleCategoriesSetKey = PrepareAccessibleCategoriesSet(tran, roles);

        return await GetCategoriesInternalAsync(tran, accessibleCategoriesSetKey);
    }

    public async Task<CacheResult<IEnumerable<Category>>> GetCategoriesAsync(string[] roles, short year)
    {
        var tran = Db.CreateTransaction();
        var accessibleCategoriesSetKey = PrepareAccessibleCategoriesSet(tran, roles);
        var accessibleCategoriesInYearSetKey = PhotoKeys.GetAccessibleCategoriesInYearSetKey(roles, year);

#pragma warning disable CS4014
        tran.SetCombineAndStoreAsync(
            SetOperation.Intersect,
            accessibleCategoriesInYearSetKey,
            accessibleCategoriesSetKey,
            PhotoKeys.GetCategoriesForYearSetKey(year)
        );
#pragma warning restore CS4014

        return await GetCategoriesInternalAsync(tran, accessibleCategoriesInYearSetKey);
    }

    // TODO: consider using a sorted set for accessible categories so we can then use zrange to simplify the logic below?
    public async Task<CacheResult<IEnumerable<Category>>> GetRecentCategoriesAsync(string[] roles, short sinceId)
    {
        var tran = Db.CreateTransaction();
        var accessibleCategoriesSetKey = PrepareAccessibleCategoriesSet(tran, roles);
        var categoryIds = tran.SetMembersAsync(accessibleCategoriesSetKey);
        var status = GetStatusAsync(tran);

        await tran.ExecuteAsync();

        var newIds = (await categoryIds)
            .Select(x => (short)x)
            .Where(x => x > sinceId);

        var theStatus = await status;

        if(!newIds.Any() || !IsStatusUsable(theStatus))
        {
            return BuildResult(
                theStatus,
                new List<Category>().AsEnumerable()
            );
        }

        tran = Db.CreateTransaction();
        var recentCategorySet = PhotoKeys.CATEGORY_RECENT_SET_KEY;

#pragma warning disable CS4014
        tran.KeyDeleteAsync(recentCategorySet);

        foreach(var id in newIds)
        {
            tran.SetAddAsync(
                recentCategorySet,
                id
            );
        }
#pragma warning restore CS4014

        return await GetCategoriesInternalAsync(tran, recentCategorySet);
    }

    public async Task<CacheResult<Category>> GetCategoryAsync(string[] roles, short categoryId)
    {
        var canAccess = await CanAccessCategoryAsync(categoryId, roles);

        if(!canAccess.ShouldUseResult || canAccess.Item == false)
        {
            return new CacheResult<Category>(canAccess.ShouldUseResult, null);
        }

        var tran = Db.CreateTransaction();

        var category = tran.HashGetAsync(
            PhotoKeys.GetCategoryHashKey(categoryId),
            _categorySerializer.HashFields
        );

        await tran.ExecuteAsync();

        return new CacheResult<Category>(true, _categorySerializer.ParseSingleOrDefault(await category));
    }

    public Task AddCategoriesAsync(IEnumerable<SecuredResource<Category>> securedCategories)
    {
        return ExecuteAsync(tran =>
        {
            foreach(var resource in securedCategories)
            {
                tran.HashSetAsync(
                    PhotoKeys.GetCategoryHashKey(resource.Item),
                    _categorySerializer.BuildHashSet(resource.Item)
                );

                tran.SetAddAsync(
                    PhotoKeys.GetCategoriesForYearSetKey(resource.Item),
                    resource.Item.Id
                );

                foreach(var role in resource.Roles) {
                    tran.SetAddAsync(
                        PhotoKeys.GetCategoriesInRoleSetKey(role),
                        resource.Item.Id
                    );
                }
            }
        });
    }

    public Task AddCategoryAsync(SecuredResource<Category> securedCategory)
    {
        return AddCategoriesAsync(new SecuredResource<Category>[] { securedCategory });
    }

    public async Task<CacheResult<IEnumerable<Photo>>> GetPhotosAsync(string[] roles, short categoryId)
    {
        var canAccess = await CanAccessCategoryAsync(categoryId, roles);

        if(!canAccess.ShouldUseResult || canAccess.Item == false)
        {
            return new CacheResult<IEnumerable<Photo>>(canAccess.ShouldUseResult, null);
        }

        var tran = Db.CreateTransaction();

        return await GetPhotosInternalAsync(tran, PhotoKeys.GetPhotosForCategorySetKey(categoryId), true);
    }

    public async Task<CacheResult<IEnumerable<Photo>>> GetRandomPhotosAsync(string[] roles, short count)
    {
        var tran = Db.CreateTransaction();
        var status = GetStatusAsync(tran);
        var accessiblePhotosSetKey = PrepareAccessiblePhotosSet(tran, roles);
        var randomPhotoIds = tran.SetRandomMembersAsync(accessiblePhotosSetKey, count);

        await tran.ExecuteAsync();

        if(!IsStatusUsable(await status))
        {
            return new CacheResult<IEnumerable<Photo>>(false, null);
        }

        tran = Db.CreateTransaction();

#pragma warning disable CS4014
        tran.KeyDeleteAsync(PhotoKeys.RANDOM_PHOTO_SET_KEY);
#pragma warning restore CS4014

        foreach(var photoId in await randomPhotoIds)
        {
#pragma warning disable CS4014
            tran.SetAddAsync(PhotoKeys.RANDOM_PHOTO_SET_KEY, photoId);
#pragma warning restore CS4014
        }

        return await GetPhotosInternalAsync(tran, PhotoKeys.RANDOM_PHOTO_SET_KEY, false);
    }

    public async Task<CacheResult<Photo>> GetPhotoAsync(string[] roles, int photoId)
    {
        var canAccess = await CanAccessPhotoAsync(photoId, roles);

        if(!canAccess.ShouldUseResult || canAccess.Item == false)
        {
            return new CacheResult<Photo>(canAccess.ShouldUseResult, null);
        }

        var tran = Db.CreateTransaction();

        var photo = tran.HashGetAsync(
            PhotoKeys.GetPhotoHashKey(photoId),
            _photoSerializer.HashFields
        );

        await tran.ExecuteAsync();

        return new CacheResult<Photo>(true, _photoSerializer.ParseSingleOrDefault(await photo));
    }

    public Task AddPhotosAsync(IEnumerable<SecuredResource<Photo>> securedPhotos)
    {
        return ExecuteAsync(tran =>
        {
            foreach(var securedPhoto in securedPhotos)
            {
                tran.HashSetAsync(
                    PhotoKeys.GetPhotoHashKey(securedPhoto.Item),
                    _photoSerializer.BuildHashSet(securedPhoto.Item)
                );

                tran.SetAddAsync(
                    PhotoKeys.GetPhotosForCategorySetKey(securedPhoto.Item.CategoryId),
                    securedPhoto.Item.Id
                );

                foreach(var role in securedPhoto.Roles) {
                    tran.SetAddAsync(
                        PhotoKeys.GetPhotosInRoleSetKey(role),
                        securedPhoto.Item.Id
                    );
                }
            }
        });
    }

    public Task AddPhotoAsync(SecuredResource<Photo> securedPhoto)
    {
        return AddPhotosAsync(new SecuredResource<Photo>[] { securedPhoto });
    }

    public async Task<Detail?> GetPhotoDetailsAsync(string[] roles, int photoId)
    {
        var canAccess = await CanAccessPhotoAsync(photoId, roles);

        if(!canAccess.ShouldUseResult || canAccess.Item == false)
        {
            return null;
        }

        var tran = Db.CreateTransaction();
        var key = PhotoKeys.GetExifHashKey(photoId);
        var exists = tran.KeyExistsAsync(key);
        var detail = tran.HashGetAsync(
            PhotoKeys.GetExifHashKey(photoId),
            _detailSerializer.HashFields
        );

        await tran.ExecuteAsync();

        if(await exists)
        {
            return _detailSerializer.ParseSingleOrDefault(await detail);
        }

        return null;
    }

    public Task AddPhotoDetailsAsync(int photoId, Detail detail)
    {
        return ExecuteAsync(tran =>
        {
            tran.HashSetAsync(
                PhotoKeys.GetExifHashKey(photoId),
                _detailSerializer.BuildHashSet(detail)
            );
        });
    }

    static string PrepareAccessibleCategoriesSet(ITransaction tran, string[] roles)
    {
        if(roles.Length == 1)
        {
            return PhotoKeys.GetCategoriesInRoleSetKey(roles[0]);
        }
        else
        {
            var accessibleCategoriesSetKey = PhotoKeys.GetCategoriesInRoleSetKey(roles);

            tran.SetCombineAndStoreAsync(
                SetOperation.Union,
                accessibleCategoriesSetKey,
                roles.Select(r => new RedisKey(PhotoKeys.GetCategoriesInRoleSetKey(r))).ToArray()
            );

            return accessibleCategoriesSetKey;
        }
    }

    static string PrepareAccessiblePhotosSet(ITransaction tran, string[] roles)
    {
        if(roles.Length == 1)
        {
            return PhotoKeys.GetPhotosInRoleSetKey(roles[0]);
        }
        else
        {
            var accessiblePhotosSetKey = PhotoKeys.GetPhotosInRoleSetKey(roles);

            tran.SetCombineAndStoreAsync(
                SetOperation.Union,
                accessiblePhotosSetKey,
                roles.Select(r => new RedisKey(PhotoKeys.GetPhotosInRoleSetKey(r))).ToArray()
            );

            return accessiblePhotosSetKey;
        }
    }

    async Task<CacheResult<IEnumerable<Category>>> GetCategoriesInternalAsync(ITransaction tran, string setKey)
    {
        var status = GetStatusAsync(tran);
        var categories = tran.SortAsync(
            setKey,
            get: _categorySerializer.SortLookupFields
        );

        await tran.ExecuteAsync();

        return BuildResult(
            await status,
            _categorySerializer.Parse(await categories)
        );
    }

    async Task<CacheResult<IEnumerable<Photo>>> GetPhotosInternalAsync(ITransaction tran, string setKey, bool sortByFilename)
    {
        var by = sortByFilename ? $"{ PhotoKeys.PHOTO_HASH_KEY_PATTERN }->xs-path" : "nosort";
        var status = GetStatusAsync(tran);
        var photos = tran.SortAsync(
            setKey,
            get: _photoSerializer.SortLookupFields,
            by: by
        );

        await tran.ExecuteAsync();

        return BuildResult(
            await status,
            _photoSerializer.Parse(await photos)
        );
    }

    async Task<CacheResult<bool>> CanAccessCategoryAsync(short categoryId, string[] roles)
    {
        var accessibleSetKeys = roles
            .Select(role => PhotoKeys.GetCategoriesInRoleSetKey(role))
            .ToArray();

        return await IsMemberOfAnySet(categoryId, accessibleSetKeys);
    }

    async Task<CacheResult<bool>> CanAccessPhotoAsync(int photoId, string[] roles)
    {
        var accessibleSetKeys = roles
            .Select(role => PhotoKeys.GetPhotosInRoleSetKey(role))
            .ToArray();

        return await IsMemberOfAnySet(photoId, accessibleSetKeys);
    }
}
