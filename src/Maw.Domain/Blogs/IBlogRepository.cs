using System.Collections.Generic;
using System.Threading.Tasks;


namespace Maw.Domain.Blogs
{
	public interface IBlogRepository
	{
		Task<List<Blog>> GetBlogsAsync();
		Task<List<Post>> GetAllPostsAsync(short blogId);
		Task<IEnumerable<Post>> GetLatestPostsAsync(short blogId, short postCount);
		Task<Post> GetPostAsync(short id);
		Task AddPostAsync(Post post);
	}
}

