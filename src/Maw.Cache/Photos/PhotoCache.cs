using StackExchange.Redis;
using Maw.Cache.Abstractions;
using Maw.Domain.Models;
using Maw.Domain.Models.Photos;

namespace Maw.Cache.Photos;

public class PhotoCache
    : BaseCache, IPhotoCache
{
    readonly CategorySerializer _categorySerializer = new();
    readonly PhotoSerializer _photoSerializer = new();

    public PhotoCache(IDatabase redisDatabase)
        : base(redisDatabase)
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
        var tran = Db.CreateTransaction();
        var accessibleCategoriesSetKey = PrepareAccessibleCategoriesSet(tran, roles);
        var singleSetKey = PhotoKeys.CATEGORY_SINGLE_SET_KEY;
        var singleAccessibleSetKey = PhotoKeys.CATEGORY_SINGLE_ACCESSIBLE_SET_KEY;

#pragma warning disable CS4014
        tran.SetAddAsync(
            singleSetKey,
            categoryId
        );

        tran.SetCombineAndStoreAsync(
            SetOperation.Intersect,
            singleAccessibleSetKey,
            singleSetKey,
            accessibleCategoriesSetKey
        );
#pragma warning restore CS4014

        var categories = await GetCategoriesInternalAsync(tran, singleAccessibleSetKey);

        return categories.Any() ? categories.First() : null;
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
        var accessibleCategoriesSetKey = PrepareAccessibleCategoriesSet(tran, roles);
        var allCats = tran.SetMembersAsync(accessibleCategoriesSetKey);

        await tran.ExecuteAsync();

        var photoSetKeys = new List<string>();

        foreach(var catId in await allCats)
        {
            photoSetKeys.Add(PhotoKeys.GetPhotosForCategorySetKey((short)catId));
        }

        tran = Db.CreateTransaction();

#pragma warning disable CS4014
        tran.KeyDeleteAsync(PhotoKeys.RANDOM_PHOTO_CANDIDATE_SET_KEY);

        tran.SetCombineAndStoreAsync(
            SetOperation.Union,
            PhotoKeys.RANDOM_PHOTO_CANDIDATE_SET_KEY,
            photoSetKeys.Select(k => new RedisKey(k)).ToArray()
        );
#pragma warning restore CS4014

        var randomKeys = tran.SetRandomMembersAsync(PhotoKeys.RANDOM_PHOTO_CANDIDATE_SET_KEY, count);

        await tran.ExecuteAsync();

        tran = Db.CreateTransaction();

#pragma warning disable CS4014
        tran.KeyDeleteAsync(PhotoKeys.RANDOM_PHOTO_SET_KEY);
#pragma warning restore CS4014

        foreach(var photoKey in await randomKeys)
        {
#pragma warning disable CS4014
            tran.SetAddAsync(PhotoKeys.RANDOM_PHOTO_SET_KEY, photoKey);
#pragma warning restore CS4014
        }

        return await GetPhotosInternalAsync(tran, PhotoKeys.RANDOM_PHOTO_SET_KEY);
    }

    public async Task<Photo?> GetPhotoAsync(string[] roles, int photoId)
    {
        var tran = Db.CreateTransaction();
        var setKey = PhotoKeys.PHOTO_SINGLE_SET_KEY;

#pragma warning disable CS4014
        tran.SetAddAsync(
            setKey,
            photoId
        );
#pragma warning restore CS4014

        var photos = await GetPhotosInternalAsync(tran, setKey);

        if(photos.Count() != 1)
        {
            return null;
        }

        var photo = photos.First();

        return await CanAccessCategoryAsync(photo.CategoryId, roles) ? photo : null;
    }

    public Task AddPhotosAsync(IEnumerable<Photo> photos)
    {
        return ExecuteAsync(tran =>
        {
            foreach(var photo in photos)
            {
                tran.HashSetAsync(
                    PhotoKeys.GetPhotoHashKey(photo),
                    _photoSerializer.BuildHashSet(photo)
                );

                tran.SetAddAsync(
                    PhotoKeys.GetPhotosForCategorySetKey(photo.CategoryId),
                    photo.Id
                );
            }
        });
    }

    public Task AddPhotoAsync(Photo photo)
    {
        return AddPhotosAsync(new Photo[] { photo });
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
        var tran = Db.CreateTransaction();
        var accessibleCategoriesSetKey = PrepareAccessibleCategoriesSet(tran, roles);
        var canAccess = tran.SetContainsAsync(accessibleCategoriesSetKey, categoryId);

        await tran.ExecuteAsync();

        return await canAccess;
    }
}
