﻿using Maw.Domain.Models.Blogs;

namespace Maw.Cache.Abstractions;

public interface IBlogCache
    : IBaseCache
{
    Task<CacheResult<IEnumerable<Blog>>> GetBlogsAsync();
    Task AddBlogsAsync(IEnumerable<Blog> blogs);
    Task AddBlogAsync(Blog blog);

    Task<CacheResult<IEnumerable<Post>>> GetPostsAsync(short blogId, long? count);
    Task AddPostsAsync(IEnumerable<Post> posts);
    Task AddPostAsync(Post post);
}
