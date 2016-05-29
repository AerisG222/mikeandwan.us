using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Maw.Domain.Blogs;
using Maw.Data.EntityFramework.Blogs;


namespace Maw.Data
{
	public class BlogRepository
		: IBlogRepository
	{
		readonly BlogContext _ctx;


		public BlogRepository(BlogContext context)
		{
			if(context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			_ctx = context;
		}


		public Task<List<Blog>> GetBlogsAsync()
		{
			return _ctx.blog
				.Select(x => new Blog {
					Id = x.id,
					Title = x.title,
					Copyright = x.copyright,
					Description = x.description,
					LastPostDate = x.post.Select(p => p.publish_date).Max(d => d)
				})
				.ToListAsync();
		}


		public Task<List<Post>> GetAllPostsAsync(short blogId)
		{
			return _ctx.post
				.Where(x => x.blog_id == blogId)
				.Select(x => BuildPost(x))
				.OrderByDescending(x => x.PublishDate)
				.ToListAsync();
		}


		public async Task<IEnumerable<Post>> GetLatestPostsAsync(short blogId, short postCount)
		{
			var posts = await _ctx.post
				.Where(x => x.blog_id == blogId)
				.OrderByDescending(x => x.publish_date)
				.Take(postCount)
				.ToListAsync();
				
			return posts.Select(x => BuildPost(x));
		}


		public Task<Post> GetPostAsync(short id)
		{
			return _ctx.post
				.Where(x => x.id == id)
				.Select(x => BuildPost(x))
				.SingleAsync();
		}


		public async Task AddPostAsync(Post post)
		{
			var p = new post()
			{
				blog_id = post.BlogId,
				title = post.Title,
				description = post.Description,
				publish_date = post.PublishDate
			};
			
			_ctx.post
				.Add(p);
			
			await _ctx.SaveChangesAsync();
		}
		
		
		Post BuildPost(post post)
		{
			return new Post {
				Id = post.id,
				BlogId = post.blog_id,
				Description = post.description,
				PublishDate = post.publish_date,
				Title = post.title
			};
		}
	}
}

