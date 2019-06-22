using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maw.Security;
using Maw.Domain.Videos;
using MawApi.Services.Videos;
using MawApi.ViewModels;
using MawApi.ViewModels.Videos;


namespace MawApi.Controllers
{
    [ApiController]
    [Authorize]
    [Authorize(Policy.ViewPhotos)]
    [Route("video-categories")]
    public class VideoCategoriesController
        : ControllerBase
    {
        readonly IVideoService _svc;
        readonly VideoAdapter _videoAdapter;
        readonly VideoCategoryAdapter _categoryAdapter;


        public VideoCategoriesController(
            IVideoService videoService,
            VideoAdapter videoAdapter,
            VideoCategoryAdapter categoryAdapter)
        {
            _svc = videoService ?? throw new ArgumentNullException(nameof(videoService));
            _videoAdapter = videoAdapter ?? throw new ArgumentNullException(nameof(videoAdapter));
            _categoryAdapter = categoryAdapter ?? throw new ArgumentNullException(nameof(categoryAdapter));
        }


        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<ApiCollection<VideoCategoryViewModel>>> GetAll()
        {
            var categories = await _svc.GetAllCategoriesAsync(Role.IsAdmin(User));
            var result = _categoryAdapter.Adapt(categories);

            return new ApiCollection<VideoCategoryViewModel>(result.ToList());
        }


        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<VideoCategoryViewModel>> GetById(short id)
        {
            var category = await _svc.GetCategoryAsync(id, Role.IsAdmin(User));

            if(category == null)
            {
                return NotFound();
            }

            return _categoryAdapter.Adapt(category);
        }


        [HttpGet("{id}/videos")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiCollection<MawApi.ViewModels.Videos.VideoViewModel>>> GetVideos(short id)
        {
            var photos = await _svc.GetVideosInCategoryAsync(id, Role.IsAdmin(User));

            if(photos == null)
            {
                return NotFound();
            }

            var results = _videoAdapter.Adapt(photos);

            return new ApiCollection<VideoViewModel>(results.ToList());
        }
    }
}
