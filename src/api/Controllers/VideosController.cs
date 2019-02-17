using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Maw.Domain;
using Maw.Domain.Videos;
using Maw.Security;
using MawApi.Models;
using MawApi.Models.Videos;
using MawApi.Services.Videos;
using MawApi.ViewModels;


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


        [HttpGet("{id}/comments")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public Task<ApiCollection<Comment>> GetCommentsAsync(int id)
        {
            return InternalGetCommentsAsync(id);
        }


        [HttpPost("{id}/comments")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ApiCollection<Comment>> AddCommentAsync(int id, CommentViewModel model)
        {
            // TODO: handle invalid photo id?
            // TODO: remove photoId from commentViewModel?
            await _svc.InsertCommentAsync(id, User.Identity.Name, model.Comment);

            return await InternalGetCommentsAsync(id);
        }


        [HttpGet("{id}/rating")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Rating>> GetRatingAsync(int id)
        {
            var rating = await InternalGetRatingAsync(id);

            if(rating == null)
            {
                return NotFound();
            }

            return rating;
        }


        [HttpPatch("{id}/rating")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Rating>> RatePhotoAsync(int id, UserRating userRating)
        {
            // TODO: handle invalid photo id?
            // TODO: remove photoId from userPhotoRating?
            if(userRating.Rating < 1)
            {
                await _svc.RemoveRatingAsync(id, User.Identity.Name);
            }
            else if(userRating.Rating <= 5)
            {
                await _svc.SaveRatingAsync(id, User.Identity.Name, userRating.Rating);
            }
            else
            {
                return BadRequest();
            }

            var rating = await InternalGetRatingAsync(id);

            if(rating == null)
            {
                return NotFound();
            }

            return rating;
        }


        async Task<ApiCollection<Comment>> InternalGetCommentsAsync(int id)
        {
            var comments = await _svc.GetCommentsAsync(id);

            return new ApiCollection<Comment>(comments.ToList());
        }


        Task<Rating> InternalGetRatingAsync(int id)
        {
            return _svc.GetRatingsAsync(id, User.Identity.Name);
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
