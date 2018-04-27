using System;
using System.Collections.Generic;
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
				return conn.QueryAsync<Blog>(
					@"SELECT *,
							 (SELECT MAX(publish_date) 
								FROM blog.post
							   WHERE blog_id = b.id
							 ) AS last_post_date 
						FROM blog.blog b;"
				);
			});
		}


		public Task<IEnumerable<Post>> GetAllPostsAsync(short blogId)
		{
			return RunAsync(conn => {
				return conn.QueryAsync<Post>(
					@"SELECT *
					    FROM blog.post
					   WHERE blog_id = @blogId
					   ORDER BY publish_date DESC;",
					new { blogId = blogId }
				);
			});
		}


		public Task<IEnumerable<Post>> GetLatestPostsAsync(short blogId, short postCount)
		{
			return RunAsync(conn => {
				return conn.QueryAsync<Post>(
					@"SELECT *
					    FROM blog.post
					   WHERE blog_id = @blogId
					   ORDER BY publish_date DESC
					   LIMIT @postCount;",
					new { 
						blogId = blogId,
					    postCount = postCount 
					}
				);
			});
		}


		public Task<Post> GetPostAsync(short id)
		{
			return RunAsync(conn => {
				return conn.QuerySingleOrDefaultAsync<Post>(
					@"SELECT *
					    FROM blog.post
					   WHERE id = @id;",
					new { id = id }
				);
			});
		}


		public Task AddPostAsync(Post post)
		{
			return RunAsync(async conn => {
				var result = await conn.ExecuteAsync(
					@"INSERT INTO blog.post
					       ( 
							 blog_id,
							 title,
							 description,
							 publish_date
						   )
					  VALUES
					       (
							 @BlogId,
							 @Title,
							 @Description,
							 @PublishDate
						   );",
					post
				);

				if(result != 1)
				{
					throw new Exception("Did not save blog post!");
				}
			});
		}
	}
}
