using Maw.Domain.Models.Blogs;

namespace Maw.Cache.Blogs;

static class BlogKeys
{
    const string ROOT = "maw:blogs";
    const string POST_ROOT = $"{ROOT}:posts";
    public const string CACHE_STATUS = $"{ROOT}:status";
    public const string BLOG_HASH_KEY_PATTERN = $"{ROOT}:*";
    public const string POST_HASH_KEY_PATTERN = $"{POST_ROOT}:*";
    public const string ALL_BLOGS_SET_KEY = $"{ROOT}:all-blogs";

    public static string GetBlogHashKey(Blog blog) => GetBlogHashKey(blog.Id);
    public static string GetBlogHashKey(short blogId) => $"{ROOT}:{blogId}";
    public static string GetBlogPostsKey(Blog blog) => GetBlogPostsKey(blog.Id);
    public static string GetBlogPostsKey(short blogId) => $"{GetBlogHashKey(blogId)}:posts";
    public static string GetPostHashKey(Post post) => GetPostHashKey(post.Id);
    public static string GetPostHashKey(short postId) => $"{POST_ROOT}:{postId}";
}
