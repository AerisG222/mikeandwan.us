﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Maw.Domain.Models.Search;
using Maw.Domain.Search;
using Maw.Security;

namespace MawApi.Controllers;

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
        ArgumentNullException.ThrowIfNull(searchService);

        _svc = searchService;
    }

    [HttpGet("multimedia-categories")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<SearchResults<MultimediaCategory>>> SearchMultimediaCategories(string query, int start = 0)
    {
        var results = await _svc.SearchAsync(User.GetAllRoles(), query, start);

        return results;
    }
}
