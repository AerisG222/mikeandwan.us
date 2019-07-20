using System;
using MawApi.Models;


namespace MawApi.Services
{
    public class UrlBuilderService
    {
        readonly UrlConfig _cfg;


        public UrlBuilderService(UrlConfig cfg)
        {
            _cfg = cfg ?? throw new ArgumentNullException(nameof(cfg));
        }


        public string BuildApiUrl(string relativePath)
        {
            if(relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            return BuildAbsoluteUrl(_cfg.Api, relativePath);
        }


        public string BuildWwwUrl(string relativePath)
        {
            if(relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            return BuildAbsoluteUrl(_cfg.Www, relativePath);
        }


        string BuildAbsoluteUrl(string host, string relativePath)
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
}
