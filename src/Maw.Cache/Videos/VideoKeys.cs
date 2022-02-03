using Maw.Domain.Models.Videos;

namespace Maw.Cache.Videos;

static class VideoKeys
{
    const string ROOT = "maw:videos";
    const string CATEGORY_ROOT = $"{ROOT}:categories";
    public const string VIDEO_HASH_KEY_PATTERN = $"{ROOT}:*";
    public const string CATEGORY_HASH_KEY_PATTERN = $"{CATEGORY_ROOT}:*";

    public static string GetVideoHashKey(Video video) => GetVideoHashKey(video.Id);
    public static string GetVideoHashKey(int videoId) => $"{ROOT}:{videoId}";
    public static string GetCategoryHashKey(Category category) => GetCategoryHashKey(category.Id);
    public static string GetCategoryHashKey(short categoryId) => $"{CATEGORY_ROOT}:{categoryId}";
    public static string GetCategoriesForYearSetKey(Category category) => GetCategoriesForYearSetKey(category.Year);
    public static string GetCategoriesForYearSetKey(short year) => $"{CATEGORY_ROOT}:years:{year}";
    public static string GetCategoriesInRoleSetKey(string role) => $"{CATEGORY_ROOT}:roles:{role}";
    public static string GetCategoriesInRoleSetKey(string[] roles) => GetCategoriesInRoleSetKey(string.Join("+", roles));
    public static string GetAccessibleCategoriesInYearSetKey(string[] roles, short year) => $"{GetCategoriesForYearSetKey(year)}:roles:{string.Join("+", roles)}";
    public static string GetVideosForCategorySetKey(Category category) => GetVideosForCategorySetKey(category.Id);
    public static string GetVideosForCategorySetKey(short categoryId) => $"{GetCategoryHashKey(categoryId)}:photos";
    public static string GetVideosInRoleSetKey(string role) => $"{ROOT}:roles:{role}";
    public static string GetVideosInRoleSetKey(string[] roles) => GetVideosInRoleSetKey(string.Join("+", roles));
}
