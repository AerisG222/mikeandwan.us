using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain.Videos;
using Maw.Security;
using MawApi.Models;
using MawApi.Services.Videos;


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
        readonly VideoAdapter _adapter;
        readonly LegacyVideoAdapter _legacyVideoAdapter;
        readonly LegacyVideoCategoryAdapter _legacyVideoCategoryAdapter;


		public VideosController(
            LegacyVideoAdapter legacyVideoAdapter,
            LegacyVideoCategoryAdapter legacyVideoCategoryAdapter,
            VideoAdapter videoAdapter,
            IVideoService videoService)
        {
			_svc = videoService ?? throw new ArgumentNullException(nameof(videoService));
            _adapter = videoAdapter ?? throw new ArgumentNullException(nameof(videoAdapter));
            _legacyVideoAdapter = legacyVideoAdapter ?? throw new ArgumentNullException(nameof(legacyVideoAdapter));
            _legacyVideoCategoryAdapter = legacyVideoCategoryAdapter ?? throw new ArgumentNullException(nameof(legacyVideoCategoryAdapter));
        }


        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MawApi.ViewModels.Videos.VideoViewModel>> GetByIdAsync(short id)
        {
            var video = await _svc.GetVideoAsync(id, Role.IsAdmin(User));

            if(video == null)
            {
                return NotFound();
            }

            return _adapter.Adapt(video);
        }


        // LEGACY APIS
        [HttpGet("getYears")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IEnumerable<short>> GetYears()
        {
            return await _svc.GetYearsAsync(Role.IsAdmin(User));
        }


        [HttpGet("getCategoriesForYear/{year:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MawApi.ViewModels.LegacyVideos.Category[]>> GetCategoriesForYear(short year)
        {
            var cats = await _svc.GetCategoriesAsync(year, Role.IsAdmin(User));

            if(cats == null || cats.Count() == 0)
            {
                return NotFound();
            }

            return _legacyVideoCategoryAdapter.Adapt(cats).ToArray();
        }


        [HttpGet("getVideosByCategory/{categoryId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MawApi.ViewModels.LegacyVideos.Video[]>> GetVideosByCategory(short categoryId)
        {
            var vids = await _svc.GetVideosInCategoryAsync(categoryId, Role.IsAdmin(User));

            if(vids == null || vids.Count() == 0)
            {
                return NotFound();
            }

            return _legacyVideoAdapter.Adapt(vids).ToArray();
        }
    }
}
