﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maw.Domain;
using Maw.Domain.Photos;
using Maw.Security;
using MawApi.Models.Photos;
using MawApi.Services.Photos;
using MawApi.ViewModels;
using MawApi.ViewModels.Photos;


namespace MawApi.Controllers
{
    [ApiController]
    [Authorize]
    [Authorize(MawPolicy.ViewPhotos)]
    [Route("photos")]
    public class PhotosController
        : ControllerBase
    {
        readonly IPhotoService _svc;
        readonly PhotoAdapter _photoAdapter;


        public PhotosController(
            IPhotoService photoService,
            PhotoAdapter photoAdapter)
        {
            _svc = photoService ?? throw new ArgumentNullException(nameof(photoService));
            _photoAdapter = photoAdapter ?? throw new ArgumentNullException(nameof(photoAdapter));
        }


        [HttpGet("random")]
        [HttpOptions("random")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<PhotoViewModel>> GetRandomPhotoAsync()
        {
            var photo = await _svc.GetRandomAsync(Role.IsAdmin(User)).ConfigureAwait(false);

            return _photoAdapter.Adapt(photo);
        }


        [HttpGet("random/{count}")]
        [HttpOptions("random/{count}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ApiCollection<PhotoViewModel>>> GetRandomPhotosAsync(byte count)
        {
            if(count > 50) {
                return BadRequest();
            }

            var photos = await _svc.GetRandomAsync(count, Role.IsAdmin(User)).ConfigureAwait(false);

            return new ApiCollection<PhotoViewModel>(_photoAdapter.Adapt(photos).ToList());
        }


        [HttpGet("{id}")]
        [HttpOptions("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<MawApi.ViewModels.Photos.PhotoViewModel>> GetByIdAsync(int id)
        {
            var photo = await _svc.GetPhotoAsync(id, Role.IsAdmin(User)).ConfigureAwait(false);

            if(photo == null)
            {
                return NotFound();
            }

            return _photoAdapter.Adapt(photo);
        }


        [HttpGet("{id}/comments")]
        [HttpOptions("{id}/comments")]
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
            if(model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // TODO: handle invalid photo id?
            // TODO: remove photoId from commentViewModel?
            await _svc.InsertCommentAsync(id, User.Identity.Name, model.Comment).ConfigureAwait(false);

            return await InternalGetCommentsAsync(id).ConfigureAwait(false);
        }


        [HttpGet("{id}/exif")]
        [HttpOptions("{id}/exif")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Detail>> GetExifAsync(int id)
        {
            var data = await _svc.GetDetailAsync(id, Role.IsAdmin(User)).ConfigureAwait(false);

            if(data == null)
            {
                return NotFound();
            }

            return data;
        }


        [HttpGet("{id}/rating")]
        [HttpOptions("{id}/rating")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Rating>> GetRatingAsync(int id)
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
        public async Task<ActionResult<Rating>> RatePhotoAsync(int id, UserRating userRating)
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


        async Task<ApiCollection<Comment>> InternalGetCommentsAsync(int id)
        {
            var comments = await _svc.GetCommentsAsync(id).ConfigureAwait(false);

            return new ApiCollection<Comment>(comments.ToList());
        }


        Task<Rating> InternalGetRatingAsync(int id)
        {
            return _svc.GetRatingsAsync(id, User.Identity.Name);
        }
    }
}
