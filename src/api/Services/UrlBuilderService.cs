using Microsoft.Extensions.Options;
using MawApi.Models;

namespace MawApi.Services;

public class UrlBuilderService
{
    readonly UrlConfig _cfg;

    public UrlBuilderService(IOptions<UrlConfig> cfg)
    {
        ArgumentNullException.ThrowIfNull(cfg.Value);

        _cfg = cfg.Value;
    }

    public string BuildApiUrl(string relativePath)
    {
        ArgumentNullException.ThrowIfNull(relativePath);

        return BuildAbsoluteUrl(_cfg.Api, relativePath);
    }

    public string BuildWwwUrl(string relativePath)
    {
        ArgumentNullException.ThrowIfNull(relativePath);

        return BuildAbsoluteUrl(_cfg.Www, relativePath);
    }

    static string BuildAbsoluteUrl(string host, string relativePath)
    {
        if(host.EndsWith('/'))
        {
            if(relativePath.StartsWith('/'))
            {
                relativePath = relativePath.Substring(1);
            }

            return $"{host}{relativePath}";
        }

        if(relativePath.StartsWith('/'))
        {
            return $"{host}{relativePath}";
        }

        return $"{host}/{relativePath}";
    }
}
