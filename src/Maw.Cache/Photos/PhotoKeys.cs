using Maw.Domain.Models.Photos;

namespace Maw.Cache.Photos;

static class PhotoKeys
{
    const string ROOT = "maw:photos";
    const string CATEGORY_ROOT = $"{ROOT}:categories";
    public const string PHOTO_HASH_KEY_PATTERN = $"{ROOT}:*";
    public const string CATEGORY_HASH_KEY_PATTERN = $"{CATEGORY_ROOT}:*";
    public const string PHOTO_SINGLE_SET_KEY = $"{ROOT}:single";
    public const string CATEGORY_SINGLE_SET_KEY = $"{CATEGORY_ROOT}:single";
    public const string CATEGORY_SINGLE_ACCESSIBLE_SET_KEY = $"{CATEGORY_ROOT}:single-accessible";
    public const string CATEGORY_ACCESSIBLE_SET_KEY = $"{CATEGORY_ROOT}:accessible";
    public const string CATEGORY_RECENT_SET_KEY = $"{CATEGORY_ROOT}:recent";
    public const string RANDOM_PHOTO_CANDIDATE_SET_KEY = $"{ROOT}:random-candidates";
    public const string RANDOM_PHOTO_SET_KEY = $"{ROOT}:random-photos";

    public static string GetPhotoHashKey(Photo photo) => GetPhotoHashKey(photo.Id);
    public static string GetPhotoHashKey(int photoId) => $"{ROOT}:{photoId}";
    public static string GetCategoryHashKey(Category category) => GetCategoryHashKey(category.Id);
    public static string GetCategoryHashKey(short categoryId) => $"{CATEGORY_ROOT}:{categoryId}";
    public static string GetCategoriesForYearSetKey(Category category) => GetCategoriesForYearSetKey(category.Year);
    public static string GetCategoriesForYearSetKey(short year) => $"{CATEGORY_ROOT}:years:{year}";
    public static string GetCategoriesInRoleSetKey(string role) => $"{CATEGORY_ROOT}:roles:{role}";
    public static string GetCategoriesInRoleSetKey(string[] roles) => GetCategoriesInRoleSetKey(string.Join("+", roles));
    public static string GetPhotosForCategorySetKey(Category category) => GetPhotosForCategorySetKey(category.Id);
    public static string GetPhotosForCategorySetKey(short categoryId) => $"{GetCategoryHashKey(categoryId)}:photos";
    public static string GetAccessibleCategoriesInYearSetKey(string[] roles, short year) => $"{GetCategoriesForYearSetKey(year)}:roles:{string.Join("+", roles)}";
}
