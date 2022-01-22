using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MawMvcApp.ViewModels;

namespace MawMvcApp;

public class MawApiService
{
    readonly ILogger _log;
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly HttpClient _client;

    public MawApiService(
        HttpClient client,
        IOptions<UrlConfig> urlConfig,
        IHttpContextAccessor httpContextAccessor,
        ILogger<MawApiService> log)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _log = log ?? throw new ArgumentNullException(nameof(log));

        if (urlConfig == null)
        {
            throw new ArgumentNullException(nameof(urlConfig));
        }

        _client.BaseAddress = new Uri(urlConfig.Value.Api);
    }

    public Task<bool> ClearPhotoCacheAsync()
    {
        return ExecuteApiCall("admin/clear-photo-cache");
    }

    public Task<bool> ClearVideoCacheAsync()
    {
        return ExecuteApiCall("admin/clear-video-cache");
    }

    async Task<bool> ExecuteApiCall(string path)
    {
        var ctx = _httpContextAccessor.HttpContext;

        if (ctx?.User == null)
        {
            throw new ArgumentException("Authenticated user is required");
        }

        var jwt = await ctx.GetTokenAsync("access_token");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, path);
            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                _log.LogInformation("Successfully cleared cache on API endpoint");

                return true;
            }
            else
            {
                _log.LogWarning("Failed to clear cache on API endpoint");

                return false;
            }
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to clear cache on API endpoint");

            return false;
        }
    }
}
