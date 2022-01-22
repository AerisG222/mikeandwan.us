using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Maw.Domain.Captcha;

//https://www.google.com/recaptcha/admin#site/318682987?setup
public class GoogleCaptchaService
    : ICaptchaService
{
    static readonly Uri URL = new Uri("https://www.google.com/recaptcha/api/siteverify");
    readonly GoogleCaptchaConfig _config;
    readonly ILogger _log;

    public GoogleCaptchaService(IOptions<GoogleCaptchaConfig> config, ILogger<GoogleCaptchaService> log)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        _config = config.Value;
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public virtual string SiteKey
    {
        get
        {
            return _config.SiteKey;
        }
    }

    public virtual async Task<bool> VerifyAsync(string recaptchaResponse)
    {
        if (string.IsNullOrEmpty(recaptchaResponse))
        {
            return false;
        }

        var parameters = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("secret", _config.SecretKey),
                new KeyValuePair<string, string>("response", recaptchaResponse)
            };

        using var client = new HttpClient();
        using var content = new FormUrlEncodedContent(parameters);
        var response = await client.PostAsync(URL, content);
        var val = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GoogleCaptchaResponse>(val)?.success ?? false;

        _log.LogDebug("google recaptcha returned: {CaptchaResult}", result);

        response.Dispose();

        return result;
    }
}
