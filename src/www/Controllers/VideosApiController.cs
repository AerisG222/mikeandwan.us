using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Videos;
using MawMvcApp.ViewModels;


namespace MawMvcApp.Controllers
{
	[Authorize(MawConstants.POLICY_VIEW_VIDEOS)]
    [Route("api/videos")]
    public class VideosApiController 
        : MawBaseController<VideosApiController>
    {
        readonly IVideoService _svc;


        bool IsAdmin
        {
            get
            {
                return User.IsInRole(MawConstants.ROLE_ADMIN);
            }
        }


		public VideosApiController(ILogger<VideosApiController> log, 
                                   IVideoService videoService)
			: base(log)
        {
			_svc = videoService ?? throw new ArgumentNullException(nameof(videoService));
        }


        [HttpGet("getYears")]
        public async Task<IEnumerable<short>> GetYears()
        {
            return await _svc.GetYearsAsync(IsAdmin);
        }


        [HttpGet("getCategoriesForYear/{year:int}")]
        public async Task<IEnumerable<Category>> GetCategoriesForYear(short year)
        {
            return await _svc.GetCategoriesAsync(year, IsAdmin);
        }


        [HttpGet("getVideosByCategory/{categoryId:int}")]
        public async Task<IEnumerable<Video>> GetVideosByCategory(short categoryId)
        {
            return await _svc.GetVideosInCategoryAsync(categoryId, IsAdmin);
        }
    }
}
