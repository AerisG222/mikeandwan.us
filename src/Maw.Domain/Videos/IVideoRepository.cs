﻿using System.Collections.Generic;
using System.Threading.Tasks;


namespace Maw.Domain.Videos
{
    public interface IVideoRepository
    {
        Task<IEnumerable<short>> GetYearsAsync(string[] roles);
        Task<IEnumerable<Category>> GetAllCategoriesAsync(string[] roles);
        Task<IEnumerable<Category>> GetCategoriesAsync(short year, string[] roles);
        Task<IEnumerable<Video>> GetVideosInCategoryAsync(short categoryId, string[] roles);
        Task<Video> GetVideoAsync(short id, string[] roles);
        Task<Category> GetCategoryAsync(short categoryId, string[] roles);
        Task<IEnumerable<Comment>> GetCommentsAsync(short videoId, string[] roles);
        Task<Rating> GetRatingsAsync(short videoId, string username, string[] roles);
        Task<GpsDetail> GetGpsDetailAsync(int videoId, string[] roles);
        Task<int> InsertCommentAsync(short videoId, string username, string comment, string[] roles);
        Task<float?> SaveRatingAsync(short videoId, string username, short rating, string[] roles);
        Task<float?> RemoveRatingAsync(short videoId, string username, string[] roles);

        // admin functions
        Task SetGpsOverrideAsync(int videoId, GpsCoordinate gps, string username);
        Task<long> SetCategoryTeaserAsync(short categoryId, int videoId);
    }
}
