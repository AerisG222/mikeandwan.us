using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maw.Domain;
using Maw.Domain.Videos;
using Maw.Security;
using MawApi.Models.Videos;
using MawApi.Services.Videos;
using MawApi.ViewModels;


namespace MawApi.Controllers
{
    [ApiController]
    [Authorize]
	[Authorize(MawPolicy.ViewVideos)]
    [Route("videos")]
    public class VideosController
        : ControllerBase
    {
        readonly IVideoService _svc;
        readonly VideoAdapter _adapter;


		public VideosController(
            VideoAdapter videoAdapter,
            IVideoService videoService)
        {
			_svc = videoService ?? throw new ArgumentNullException(nameof(videoService));
            _adapter = videoAdapter ?? throw new ArgumentNullException(nameof(videoAdapter));
        }


        [HttpGet("{id}")]
        [HttpOptions("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MawApi.ViewModels.Videos.VideoViewModel>> GetByIdAsync(short id)
        {
            var video = await _svc.GetVideoAsync(id, Role.IsAdmin(User)).ConfigureAwait(false);

            if(video == null)
            {
                return NotFound();
            }

            return _adapter.Adapt(video);
        }


        [HttpGet("{id}/comments")]
        [HttpOptions("{id}/comments")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public Task<ApiCollection<Comment>> GetCommentsAsync(short id)
        {
            return InternalGetCommentsAsync(id);
        }


        [HttpPost("{id}/comments")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ApiCollection<Comment>> AddCommentAsync(short id, CommentViewModel model)
        {
            if(model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // TODO: handle invalid photo id?
            // TODO: remove photoId from commentViewModel?
            await _svc.InsertCommentAsync(id, User.Identity.Name, model.Comment).ConfigureAwait(false);

            return await InternalGetCommentsAsync(id).ConfigureAwait(false);
        }


        [HttpGet("{id}/rating")]
        [HttpOptions("{id}/rating")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Rating>> GetRatingAsync(short id)
        {
            var rating = await InternalGetRatingAsync(id).ConfigureAwait(false);

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
        public async Task<ActionResult<Rating>> RateVideoAsync(short id, UserRating userRating)
        {
            if(userRating == null)
            {
                throw new ArgumentNullException(nameof(userRating));
            }

            // TODO: handle invalid photo id?
            // TODO: remove photoId from userPhotoRating?
            if(userRating.Rating < 1)
            {
                await _svc.RemoveRatingAsync(id, User.Identity.Name).ConfigureAwait(false);
            }
            else if(userRating.Rating <= 5)
            {
                await _svc.SaveRatingAsync(id, User.Identity.Name, userRating.Rating).ConfigureAwait(false);
            }
            else
            {
                return BadRequest();
            }

            var rating = await InternalGetRatingAsync(id).ConfigureAwait(false);

            if(rating == null)
            {
                return NotFound();
            }

            return rating;
        }


        async Task<ApiCollection<Comment>> InternalGetCommentsAsync(short id)
        {
            var comments = await _svc.GetCommentsAsync(id).ConfigureAwait(false);

            return new ApiCollection<Comment>(comments.ToList());
        }


        Task<Rating> InternalGetRatingAsync(short id)
        {
            return _svc.GetRatingsAsync(id, User.Identity.Name);
        }
    }
}
