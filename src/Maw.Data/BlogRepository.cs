using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Maw.Domain.Blogs;

namespace Maw.Data;

public class BlogRepository
    : Repository, IBlogRepository
{
    public BlogRepository(string connectionString)
        : base(connectionString)
    {

    }

    public Task<IEnumerable<Blog>> GetBlogsAsync()
    {
        return RunAsync(conn =>
            conn.QueryAsync<Blog>("SELECT * FROM blog.get_blogs();")
        );
    }

    public Task<IEnumerable<Post>> GetAllPostsAsync(short blogId)
    {
        return InternalGetPostsAsync(blogId);
    }

    public Task<IEnumerable<Post>> GetLatestPostsAsync(short blogId, short postCount)
    {
        return InternalGetPostsAsync(blogId, postCount: postCount);
    }

    public async Task<Post> GetPostAsync(short id)
    {
        var result = await InternalGetPostsAsync(postId: id);

        return result.FirstOrDefault();
    }

    public Task AddPostAsync(Post post)
    {
        return RunAsync(async conn =>
        {
            var result = await conn.QuerySingleOrDefaultAsync<short>(
                @"SELECT * FROM blog.add_post(
                    @blogId,
                    @title,
                    @description,
                    @publishDate
                );",
                new
                {
                    blogId = post.BlogId,
                    title = post.Title,
                    description = post.Description,
                    publishDate = post.PublishDate
                });

            if (result <= 0)
            {
                throw new Exception("Did not save blog post!");
            }
        });
    }

    Task<IEnumerable<Post>> InternalGetPostsAsync(short? blogId = null, short? postId = null, short? postCount = null)
    {
        return RunAsync(conn =>
            conn.QueryAsync<Post>(
                "SELECT * FROM blog.get_posts(@blogId, @postId, @postCount);",
                new
                {
                    blogId,
                    postId,
                    postCount
                }
            )
        );
    }
}
