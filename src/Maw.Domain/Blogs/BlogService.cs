using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;


namespace Maw.Domain.Blogs
{
	public class BlogService
		: IBlogService
	{
        const string CachePrefix = "blog";
        readonly TimeSpan _maxCacheTime = TimeSpan.FromDays(365);
		readonly IBlogRepository _repo;
        readonly IDistributedCache _cache;


		public BlogService(IBlogRepository blogRepository,
                           IDistributedCache cache)
		{
			_repo = blogRepository ?? throw new ArgumentNullException(nameof(blogRepository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
		}


		public Task<IEnumerable<Blog>> GetBlogsAsync()
		{
            var key = $"{CachePrefix}_{nameof(GetBlogsAsync)}";

            return GetOrSetValue(key, () => _repo.GetBlogsAsync(), _maxCacheTime);
		}


		public Task<IEnumerable<Post>> GetAllPostsAsync(short blogId)
		{
            var key = BuildAllPostsKey(blogId);

			return GetOrSetValue(key, () => _repo.GetAllPostsAsync(blogId), _maxCacheTime);
		}


		public async Task<IEnumerable<Post>> GetLatestPostsAsync(short blogId, short postCount)
		{
			// return _repo.GetLatestPostsAsync(blogId, postCount);
            var posts = await GetAllPostsAsync(blogId).ConfigureAwait(false);

            return posts
                .OrderByDescending(p => p.PublishDate)
                .Take(postCount);
		}


		public Task AddPostAsync(Post post)
		{
            if(post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            var key = BuildAllPostsKey(post.BlogId);

            return Task.WhenAll(
                _repo.AddPostAsync(post),
                _cache.RemoveAsync(key)
            );
		}


        string BuildAllPostsKey(short blogId) {
            return $"{CachePrefix}_{nameof(GetAllPostsAsync)}_{blogId}";
        }


        async Task<T> GetOrSetValue<T>(string key, Func<Task<T>> func, TimeSpan slidingExpiration)
        {
            var cachedValue = await _cache.GetAsync(key).ConfigureAwait(false);

            if(cachedValue == null)
            {
                var result = await func().ConfigureAwait(false);
                var opts = new DistributedCacheEntryOptions { SlidingExpiration = slidingExpiration };

                await _cache.SetStringAsync(key, JsonSerializer.Serialize(result), opts).ConfigureAwait(false);

                return result;
            }

            return JsonSerializer.Deserialize<T>(cachedValue);
        }
	}
}

