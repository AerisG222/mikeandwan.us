using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Maw.Domain.Blogs
{
	public class BlogService
	{
		readonly IBlogRepository _repo;


		public BlogService(IBlogRepository blogRepository)
		{
			if(blogRepository == null)
			{
				throw new ArgumentNullException(nameof(blogRepository));
			}

			_repo = blogRepository;
		}


		public Task<IEnumerable<Blog>> GetBlogsAsync()
		{
			return _repo.GetBlogsAsync();
		}


		public Task<IEnumerable<Post>> GetAllPostsAsync(byte blogId)
		{
			return _repo.GetAllPostsAsync(blogId);
		}


		public Task<IEnumerable<Post>> GetLatestPostsAsync(byte blogId, short postCount)
		{
			return _repo.GetLatestPostsAsync(blogId, postCount);
		}
		
		
		public Task AddPostAsync(Post post)
		{
			return _repo.AddPostAsync(post);
		}
	}
}

