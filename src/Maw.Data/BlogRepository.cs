using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using D = Maw.Domain.Blogs;
using Maw.Data.EntityFramework.Blogs;


namespace Maw.Data
{
	public class BlogRepository
		: D.IBlogRepository
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


		public Task<List<D.Blog>> GetBlogsAsync()
		{
			return _ctx.Blog
				.Select(x => new D.Blog 
				{
					Id = x.Id,
					Title = x.Title,
					Copyright = x.Copyright,
					Description = x.Description,
					LastPostDate = x.Post.Select(p => p.PublishDate).Max(d => d)
				})
				.ToListAsync();
		}


		public Task<List<D.Post>> GetAllPostsAsync(short blogId)
		{
			return _ctx.Post
				.Where(x => x.BlogId == blogId)
				.Select(x => BuildPost(x))
				.OrderByDescending(x => x.PublishDate)
				.ToListAsync();
		}


		public async Task<IEnumerable<D.Post>> GetLatestPostsAsync(short blogId, short postCount)
		{
			var posts = await _ctx.Post
				.Where(x => x.BlogId == blogId)
				.OrderByDescending(x => x.PublishDate)
				.Take(postCount)
				.ToListAsync();
				
			return posts.Select(x => BuildPost(x));
		}


		public Task<D.Post> GetPostAsync(short id)
		{
			return _ctx.Post
				.Where(x => x.Id == id)
				.Select(x => BuildPost(x))
				.SingleAsync();
		}


		public async Task AddPostAsync(D.Post post)
		{
			var p = new Post
			{
				BlogId = post.BlogId,
				Title = post.Title,
				Description = post.Description,
				PublishDate = post.PublishDate
			};
			
			_ctx.Post
				.Add(p);
			
			await _ctx.SaveChangesAsync();
		}
		
		
		D.Post BuildPost(Post post)
		{
			return new D.Post 
			{
				Id = post.Id,
				BlogId = post.BlogId,
				Description = post.Description,
				PublishDate = post.PublishDate,
				Title = post.Title
			};
		}
	}
}

