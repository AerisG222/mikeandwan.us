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
        : base(redisDatabase, VideoKeys.CACHE_STATUS)
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

    public async Task<CacheResult<Category>> GetCategoryAsync(string[] roles, short categoryId)
    {
        var canAccess = await CanAccessCategoryAsync(categoryId, roles);

        if(!canAccess.ShouldUseResult || canAccess.Item == false)
        {
            return new CacheResult<Category>(canAccess.ShouldUseResult, null);
        }

        var tran = Db.CreateTransaction();

        var category = tran.HashGetAsync(
            VideoKeys.GetCategoryHashKey(categoryId),
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

    public async Task<CacheResult<IEnumerable<Video>>> GetVideosAsync(string[] roles, short categoryId)
    {
        var canAccess = await CanAccessCategoryAsync(categoryId, roles);

        if(!canAccess.ShouldUseResult || canAccess.Item == false)
        {
            return new CacheResult<IEnumerable<Video>>(canAccess.ShouldUseResult, null);
        }

        var tran = Db.CreateTransaction();

        return await GetVideosInternalAsync(tran, VideoKeys.GetVideosForCategorySetKey(categoryId));
    }

    public async Task<CacheResult<Video>> GetVideoAsync(string[] roles, int videoId)
    {
        var canAccess = await CanAccessVideoAsync(videoId, roles);

        if(!canAccess.ShouldUseResult || canAccess.Item == false)
        {
            return new CacheResult<Video>(canAccess.ShouldUseResult, null);
        }

        var tran = Db.CreateTransaction();

        var video = tran.HashGetAsync(
            VideoKeys.GetVideoHashKey(videoId),
            _videoSerializer.HashFields
        );

        await tran.ExecuteAsync();

        return new CacheResult<Video>(true, _videoSerializer.ParseSingleOrDefault(await video));
    }

    public Task AddVideosAsync(IEnumerable<SecuredResource<Video>> securedVideos)
    {
        return ExecuteAsync(tran =>
        {
            foreach(var securedVideo in securedVideos)
            {
                tran.HashSetAsync(
                    VideoKeys.GetVideoHashKey(securedVideo.Item),
                    _videoSerializer.BuildHashSet(securedVideo.Item)
                );

                tran.SetAddAsync(
                    VideoKeys.GetVideosForCategorySetKey(securedVideo.Item.CategoryId),
                    securedVideo.Item.Id
                );

                foreach(var role in securedVideo.Roles) {
                    tran.SetAddAsync(
                        VideoKeys.GetVideosInRoleSetKey(role),
                        securedVideo.Item.Id
                    );
                }
            }
        });
    }

    public Task AddVideoAsync(SecuredResource<Video> securedVideo)
    {
        return AddVideosAsync(new SecuredResource<Video>[] { securedVideo });
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

    async Task<CacheResult<IEnumerable<Video>>> GetVideosInternalAsync(ITransaction tran, string setKey)
    {
        var status = GetStatusAsync(tran);
        var photos = tran.SortAsync(
            setKey,
            get: _videoSerializer.SortLookupFields
        );

        await tran.ExecuteAsync();

        return BuildResult(
            await status,
            _videoSerializer.Parse(await photos)
        );
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

    async Task<CacheResult<bool>> CanAccessCategoryAsync(short categoryId, string[] roles)
    {
        var accessibleSetKeys = roles
            .Select(role => VideoKeys.GetCategoriesInRoleSetKey(roles))
            .ToArray();

        return await IsMemberOfAnySet(categoryId, accessibleSetKeys);
    }

    async Task<CacheResult<bool>> CanAccessVideoAsync(int videoId, string[] roles)
    {
        var accessibleSetKeys = roles
            .Select(role => VideoKeys.GetVideosInRoleSetKey(role))
            .ToArray();

        return await IsMemberOfAnySet(videoId, accessibleSetKeys);
    }
}
