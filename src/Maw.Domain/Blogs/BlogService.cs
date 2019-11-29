using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;


namespace Maw.Domain.Blogs
{
	public class BlogService
		: BaseService, IBlogService
	{
		readonly IBlogRepository _repo;


		public BlogService(IBlogRepository blogRepository,
                           ILogger<BlogService> log,
                           IDistributedCache cache)
            : base("blog", log, cache)
		{
			_repo = blogRepository ?? throw new ArgumentNullException(nameof(blogRepository));
		}


		public Task<IEnumerable<Blog>> GetBlogsAsync()
		{
            return GetCachedValueAsync(nameof(GetBlogsAsync), () => _repo.GetBlogsAsync());
		}


		public Task<IEnumerable<Post>> GetAllPostsAsync(short blogId)
		{
            var key = $"{nameof(GetAllPostsAsync)}_{blogId}";

			return GetCachedValueAsync(key, () => _repo.GetAllPostsAsync(blogId));
		}


		public Task<IEnumerable<Post>> GetLatestPostsAsync(short blogId, short postCount)
		{
            var key = $"{nameof(GetLatestPostsAsync)}_{blogId}_{postCount}";

            return GetCachedValueAsync(key, () => _repo.GetLatestPostsAsync(blogId, postCount));
		}


		public Task AddPostAsync(Post post)
		{
            if(post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            return Task.WhenAll(
                _repo.AddPostAsync(post),
                ClearCacheAsync()
            );
		}
	}
}
