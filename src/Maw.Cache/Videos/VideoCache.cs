using StackExchange.Redis;
using Maw.Cache.Abstractions;
using Maw.Domain.Models;
using Maw.Domain.Models.Videos;

namespace Maw.Cache.Videos;

public class VideoCache
    : BaseCache, IVideoCache
{
    readonly CategorySerializer _categorySerializer = new();
    readonly VideoSerializer _videoSerializer = new();

    public VideoCache(IDatabase redisDatabase)
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
        var accessibleCategoriesInYearSetKey = VideoKeys.GetAccessibleCategoriesInYearSetKey(roles, year);

#pragma warning disable CS4014
        tran.SetCombineAndStoreAsync(
            SetOperation.Intersect,
            accessibleCategoriesInYearSetKey,
            accessibleCategoriesSetKey,
            VideoKeys.GetCategoriesForYearSetKey(year)
        );
#pragma warning restore CS4014

        return await GetCategoriesInternalAsync(tran, accessibleCategoriesInYearSetKey);
    }

    public async Task<Category?> GetCategoryAsync(string[] roles, short categoryId)
    {
        var tran = Db.CreateTransaction();
        var accessibleCategoriesSetKey = PrepareAccessibleCategoriesSet(tran, roles);
        var singleSetKey = VideoKeys.CATEGORY_SINGLE_SET_KEY;
        var singleAccessibleSetKey = VideoKeys.CATEGORY_SINGLE_ACCESSIBLE_SET_KEY;

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
                    VideoKeys.GetCategoryHashKey(resource.Item),
                    _categorySerializer.BuildHashSet(resource.Item)
                );

                tran.SetAddAsync(
                    VideoKeys.GetCategoriesForYearSetKey(resource.Item),
                    resource.Item.Id
                );

                foreach(var role in resource.Roles) {
                    tran.SetAddAsync(
                        VideoKeys.GetCategoriesInRoleSetKey(role),
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

    public async Task<IEnumerable<Video>> GetVideosAsync(string[] roles, short categoryId)
    {
        if(await CanAccessCategoryAsync(categoryId, roles))
        {
            var tran = Db.CreateTransaction();

            return await GetVideosInternalAsync(tran, VideoKeys.GetVideosForCategorySetKey(categoryId));
        }

        return new List<Video>();
    }

    public async Task<Video?> GetVideoAsync(string[] roles, short videoId)
    {
        var tran = Db.CreateTransaction();
        var setKey = VideoKeys.VIDEO_SINGLE_SET_KEY;

#pragma warning disable CS4014
        tran.SetAddAsync(
            setKey,
            videoId
        );
#pragma warning restore CS4014

        var videos = await GetVideosInternalAsync(tran, setKey);

        if(videos.Count() != 1)
        {
            return null;
        }

        var video = videos.First();

        return await CanAccessCategoryAsync(video.CategoryId, roles) ? video : null;
    }

    public Task AddVideosAsync(IEnumerable<Video> videos)
    {
        return ExecuteAsync(tran =>
        {
            foreach(var video in videos)
            {
                tran.HashSetAsync(
                    VideoKeys.GetVideoHashKey(video),
                    _videoSerializer.BuildHashSet(video)
                );

                tran.SetAddAsync(
                    VideoKeys.GetVideosForCategorySetKey(video.CategoryId),
                    video.Id
                );
            }
        });
    }

    public Task AddVideoAsync(Video video)
    {
        return AddVideosAsync(new Video[] { video });
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

    async Task<IEnumerable<Video>> GetVideosInternalAsync(ITransaction tran, string setKey)
    {
        var photos = tran.SortAsync(
            setKey,
            get: _videoSerializer.SortLookupFields
        );

        await tran.ExecuteAsync();

        return _videoSerializer.Parse(await photos);
    }

    static string PrepareAccessibleCategoriesSet(ITransaction tran, string[] roles)
    {
        if(roles.Length == 1)
        {
            return VideoKeys.GetCategoriesInRoleSetKey(roles[0]);
        }
        else
        {
            var accessibleCategoriesSetKey = VideoKeys.GetCategoriesInRoleSetKey(roles);

            tran.SetCombineAndStoreAsync(
                SetOperation.Union,
                accessibleCategoriesSetKey,
                roles.Select(r => new RedisKey(VideoKeys.GetCategoriesInRoleSetKey(r))).ToArray()
            );

            return accessibleCategoriesSetKey;
        }
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
