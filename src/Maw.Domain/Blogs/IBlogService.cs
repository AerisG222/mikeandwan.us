using Maw.Domain.Models.Blogs;

namespace Maw.Domain.Blogs;

public interface IBlogService
{
    Task<IEnumerable<Blog>> GetBlogsAsync();
    Task<IEnumerable<Post>> GetAllPostsAsync(short blogId);
    Task<IEnumerable<Post>> GetLatestPostsAsync(short blogId, short postCount);
    Task AddPostAsync(Post post);
}
