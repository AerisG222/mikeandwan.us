using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolrNet;
using Maw.Domain.Search;
using Maw.Security;


namespace MawApi.Controllers
{
    [ApiController]
    [Authorize]
    [Authorize(MawPolicy.ViewPhotos)]
    [Authorize(MawPolicy.ViewVideos)]
    [Route("search")]
    public class SearchController
        : ControllerBase
    {
        readonly ISearchService _svc;


        public SearchController(
            ISearchService searchService)
        {
            _svc = searchService ?? throw new ArgumentNullException(nameof(searchService));
        }


        [HttpGet("multimedia-categories")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<SolrQueryResults<MultimediaCategory>>> SearchMultimediaCategories(string query, int start = 0)
        {
            var results = await _svc.SearchAsync(Role.IsAdmin(User), query, start).ConfigureAwait(false);

            return results;
        }
    }
}
