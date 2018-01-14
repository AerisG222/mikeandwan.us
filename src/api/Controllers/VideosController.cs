using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Videos;
using MawApi.ViewModels;
using Maw.Security;
using Maw.Security.Filters;


namespace MawApi.Controllers
{
	[Authorize(Policy.ViewVideos)]
    [Route("videos")]
    public class VideosController
        : ControllerBase
    {
        readonly IVideoService _svc;


		public VideosController(IVideoService videoService)
        {
			_svc = videoService ?? throw new ArgumentNullException(nameof(videoService));
        }


        [HttpGet("getYears")]
        public async Task<IEnumerable<short>> GetYears()
        {
            return await _svc.GetYearsAsync(Role.IsAdmin(User));
        }


        [HttpGet("getCategoriesForYear/{year:int}")]
        public async Task<IEnumerable<Category>> GetCategoriesForYear(short year)
        {
            return await _svc.GetCategoriesAsync(year, Role.IsAdmin(User));
        }


        [HttpGet("getVideosByCategory/{categoryId:int}")]
        public async Task<IEnumerable<Video>> GetVideosByCategory(short categoryId)
        {
            return await _svc.GetVideosInCategoryAsync(categoryId, Role.IsAdmin(User));
        }
    }
}
