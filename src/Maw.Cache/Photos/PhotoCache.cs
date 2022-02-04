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

    public async Task<IEnumerable<short>> GetYearsAsync(string[] roles)
    {
        var tran = Db.CreateTransaction();
        var accessibleCategoriesSetKey = PrepareAccessibleCategoriesSet(tran, roles);

        var years = tran.SortAsync(
            accessibleCategoriesSetKey,
            get: CategorySerializer.YearLookupField
        );

        await tran.ExecuteAsync();

        return (await years)
            .Select(year => (short)year)
            .Distinct()
            .OrderByDescending(year => year);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(string[] roles)
    {
        var tran = Db.CreateTransaction();
        var accessibleCategoriesSetKey = PrepareAccessibleCategoriesSet(tran, roles);

        return await GetCategoriesInternalAsync(tran, accessibleCategoriesSetKey);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(string[] roles, short year)
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
    public async Task<IEnumerable<Category>> GetRecentCategoriesAsync(string[] roles, short sinceId)
    {
        var tran = Db.CreateTransaction();
        var accessibleCategoriesSetKey = PrepareAccessibleCategoriesSet(tran, roles);
        var categoryIds = tran.SetMembersAsync(accessibleCategoriesSetKey);

        await tran.ExecuteAsync();

        var newIds = (await categoryIds)
            .Select(x => (short)x)
            .Where(x => x > sinceId);

        if(!newIds.Any())
        {
            return new List<Category>();
        }

        tran = Db.CreateTransaction();
        var recentCategorySet = PhotoKeys.CATEGORY_RECENT_SET_KEY;

        foreach(var id in newIds)
        {
#pragma warning disable CS4014
            tran.SetAddAsync(
                recentCategorySet,
                id
            );
#pragma warning restore CS4014
        }

        return await GetCategoriesInternalAsync(tran, recentCategorySet);
    }

    public async Task<Category?> GetCategoryAsync(string[] roles, short categoryId)
    {
        if(!await CanAccessCategoryAsync(categoryId, roles))
        {
            return null;
        }

        var tran = Db.CreateTransaction();

        var category = tran.HashGetAsync(
            PhotoKeys.GetCategoryHashKey(categoryId),
            _categorySerializer.HashFields
        );

        await tran.ExecuteAsync();

        return _categorySerializer.ParseSingleOrDefault(await category);
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

    public async Task<IEnumerable<Photo>> GetPhotosAsync(string[] roles, short categoryId)
    {
        if(await CanAccessCategoryAsync(categoryId, roles))
        {
            var tran = Db.CreateTransaction();

            return await GetPhotosInternalAsync(tran, PhotoKeys.GetPhotosForCategorySetKey(categoryId));
        }

        return new List<Photo>();
    }

    public async Task<IEnumerable<Photo>> GetRandomPhotosAsync(string[] roles, short count)
    {
        var tran = Db.CreateTransaction();
        var accessiblePhotosSetKey = PrepareAccessiblePhotosSet(tran, roles);
        var randomPhotoIds = tran.SetRandomMembersAsync(accessiblePhotosSetKey, count);

        await tran.ExecuteAsync();

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

        return await GetPhotosInternalAsync(tran, PhotoKeys.RANDOM_PHOTO_SET_KEY);
    }

    public async Task<Photo?> GetPhotoAsync(string[] roles, int photoId)
    {
        if(!await CanAccessPhotoAsync(photoId, roles))
        {
            return null;
        }

        var tran = Db.CreateTransaction();

        var photo = tran.HashGetAsync(
            PhotoKeys.GetPhotoHashKey(photoId),
            _photoSerializer.HashFields
        );

        await tran.ExecuteAsync();

        return _photoSerializer.ParseSingleOrDefault(await photo);
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
        if(!await CanAccessPhotoAsync(photoId, roles))
        {
            return null;
        }

        var tran = Db.CreateTransaction();

        var detail = tran.HashGetAsync(
            PhotoKeys.GetExifHashKey(photoId),
            _detailSerializer.HashFields
        );

        await tran.ExecuteAsync();

        return _detailSerializer.ParseSingleOrDefault(await detail);
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

    async Task<IEnumerable<Category>> GetCategoriesInternalAsync(ITransaction tran, string setKey)
    {
        var categories = tran.SortAsync(
            setKey,
            get: _categorySerializer.SortLookupFields
        );

        await tran.ExecuteAsync();

        return _categorySerializer.Parse(await categories);
    }

    async Task<IEnumerable<Photo>> GetPhotosInternalAsync(ITransaction tran, string setKey)
    {
        var photos = tran.SortAsync(
            setKey,
            get: _photoSerializer.SortLookupFields
        );

        await tran.ExecuteAsync();

        return _photoSerializer.Parse(await photos);
    }

    async Task<bool> CanAccessCategoryAsync(short categoryId, string[] roles)
    {
        var accessibleSetKeys = roles
            .Select(role => PhotoKeys.GetCategoriesInRoleSetKey(role))
            .ToArray();

        return await IsMemberOfAnySet(categoryId, accessibleSetKeys);
    }

    async Task<bool> CanAccessPhotoAsync(int photoId, string[] roles)
    {
        var accessibleSetKeys = roles
            .Select(role => PhotoKeys.GetPhotosInRoleSetKey(role))
            .ToArray();

        return await IsMemberOfAnySet(photoId, accessibleSetKeys);
    }
}
