using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Maw.Cache.Abstractions;

namespace Maw.Cache.Tests;

public class BlogTests
{
    [Fact]
    public async Task GetBlogs_CacheMatchesData()
    {
        var dbBlogs = await TestHelper.BlogRepository.GetBlogsAsync();

        await TestHelper.BlogCache.AddBlogsAsync(dbBlogs);
        await TestHelper.BlogCache.SetStatusAsync(CacheStatus.InitializationSucceeded);

        var cacheBlogs = (await TestHelper.BlogCache.GetBlogsAsync()).Value;

        Assert.Equal(dbBlogs.Count(), cacheBlogs.Count());

        var dbFirst = dbBlogs.First();
        var cacheFirst = cacheBlogs.First();

        Assert.Equal(dbFirst.Copyright, cacheFirst.Copyright);
        Assert.Equal(dbFirst.Description, cacheFirst.Description);
        Assert.Equal(dbFirst.Id, cacheFirst.Id);
        Assert.Equal(dbFirst.LastPostDate, cacheFirst.LastPostDate);
        Assert.Equal(dbFirst.Title, cacheFirst.Title);
    }

    [Fact]
    public async Task GetPosts_CacheMatchesData()
    {
        short blogId = 1;
        var dbPosts = await TestHelper.BlogRepository.GetAllPostsAsync(blogId);

        await TestHelper.BlogCache.AddPostsAsync(dbPosts);
        await TestHelper.BlogCache.SetStatusAsync(CacheStatus.InitializationSucceeded);

        var cachePosts = (await TestHelper.BlogCache.GetPostsAsync(blogId)).Value;

        Assert.Equal(dbPosts.Count(), cachePosts.Count());

        var dbFirst = dbPosts.First();
        var cacheFirst = cachePosts.First();

        Assert.Equal(dbFirst.BlogId, cacheFirst.BlogId);
        Assert.Equal(dbFirst.Description, cacheFirst.Description);
        Assert.Equal(dbFirst.Id, cacheFirst.Id);
        Assert.Equal(dbFirst.PublishDate, cacheFirst.PublishDate);
        Assert.Equal(dbFirst.Title, cacheFirst.Title);
    }

    [Fact]
    public async Task GetPostsLimited_CacheMatchesData()
    {
        short blogId = 1;
        var dbPosts = await TestHelper.BlogRepository.GetAllPostsAsync(blogId);

        await TestHelper.BlogCache.AddPostsAsync(dbPosts);
        await TestHelper.BlogCache.SetStatusAsync(CacheStatus.InitializationSucceeded);

        var cachePosts = (await TestHelper.BlogCache.GetPostsAsync(blogId)).Value;

        Assert.Equal(dbPosts.Count(), cachePosts.Count());

        var dbFirst = dbPosts.First();
        var cacheFirst = cachePosts.First();

        Assert.Equal(dbFirst.BlogId, cacheFirst.BlogId);
        Assert.Equal(dbFirst.Description, cacheFirst.Description);
        Assert.Equal(dbFirst.Id, cacheFirst.Id);
        Assert.Equal(dbFirst.PublishDate, cacheFirst.PublishDate);
        Assert.Equal(dbFirst.Title, cacheFirst.Title);
    }

    [Fact]
    public async Task VerifyCacheStatus()
    {
        await TestHelper.BlogCache.SetStatusAsync(CacheStatus.UnInitialized);

        var status = await TestHelper.BlogCache.GetStatusAsync();

        Assert.Equal(CacheStatus.UnInitialized, status);

        var blogs = await TestHelper.BlogCache.GetBlogsAsync();

        Assert.False(blogs.ShouldUseResult);

        await TestHelper.BlogCache.SetStatusAsync(CacheStatus.InitializationSucceeded);

        status = await TestHelper.BlogCache.GetStatusAsync();

        Assert.Equal(CacheStatus.InitializationSucceeded, status);

        blogs = await TestHelper.BlogCache.GetBlogsAsync();

        Assert.True(blogs.ShouldUseResult);
    }
}
