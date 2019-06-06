using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Maw.Domain.Blogs;


namespace Maw.Data
{
	public class BlogRepository
		: Repository, IBlogRepository
	{
		public BlogRepository(string connectionString)
			: base(connectionString)
		{

		}


		public Task<IEnumerable<Blog>> GetBlogsAsync()
		{
			return RunAsync(conn => {
				return conn.QueryAsync<Blog>("SELECT * FROM blog.get_blogs();");
			});
		}


		public Task<IEnumerable<Post>> GetAllPostsAsync(short blogId)
		{
			return RunAsync(conn => {
				return conn.QueryAsync<Post>(
                    "SELECT * FROM blog.get_posts(@blogId);",
					new { blogId = blogId }
				);
			});
		}


		public Task<IEnumerable<Post>> GetLatestPostsAsync(short blogId, short postCount)
		{
            return RunAsync(conn => {
				return conn.QueryAsync<Post>(
                    "SELECT * FROM blog.get_posts(@blogId, @id, @postCount);",
					new {
						blogId = blogId,
                        id = (short?) null,
					    postCount = postCount
					}
				);
			});
		}


		public Task<Post> GetPostAsync(short id)
		{
			return RunAsync(conn => {
				return conn.QuerySingleOrDefaultAsync<Post>(
					@"SELECT * FROM blog.get_posts(@blogId, @id);",
					new {
                        blogId = (short?) null,
                        id = id
                    }
				);
			});
		}


		public Task AddPostAsync(Post post)
		{
			return RunAsync(async conn => {
				var result = await conn.QuerySingleOrDefaultAsync<short>(
					@"SELECT * FROM blog.add_post(
                        @blogId,
                        @title,
                        @description,
                        @publishDate
                    );",
					new {
                        blogId = post.BlogId,
                        title = post.Title,
                        description = post.Description,
                        publishDate = post.PublishDate
                    });

				if(result <= 0)
				{
					throw new Exception("Did not save blog post!");
				}
			});
		}
	}
}
