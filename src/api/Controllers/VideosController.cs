using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Videos;
using MawApi.ViewModels;
using Maw.Security;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace MawApi.Controllers
{
    [ApiController]
    [Authorize]
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
        public async Task<ActionResult<Category[]>> GetCategoriesForYear(short year)
        {
            var cats = await _svc.GetCategoriesAsync(year, Role.IsAdmin(User));

            if(cats == null || cats.Count() == 0)
            {
                return NotFound();
            }

            return cats.ToArray();
        }


        [HttpGet("getVideosByCategory/{categoryId:int}")]
        public async Task<ActionResult<Video[]>> GetVideosByCategory(short categoryId)
        {
            var vids = await _svc.GetVideosInCategoryAsync(categoryId, Role.IsAdmin(User));

            if(vids == null || vids.Count() == 0)
            {
                return NotFound();
            }

            return vids.ToArray();
        }
    }
}
