using Microsoft.FeatureManagement;

namespace Maw.Domain.Captcha;

public class CaptchaFeature
    : ICaptchaFeature
{
    readonly IFeatureManager _featureManager;
    readonly IEnumerable<ICaptchaService> _captchaServices;
    ICaptchaService? _service;

    public CaptchaFeature(
        IFeatureManager featureManager,
        IEnumerable<ICaptchaService> captchaServices
    ) {
        _featureManager = featureManager ?? throw new ArgumentNullException(nameof(featureManager));
        _captchaServices = captchaServices ?? throw new ArgumentNullException(nameof(captchaServices));
    }

    public async Task<ICaptchaService> GetServiceAsync()
    {
        if(_service != null)
        {
            return _service;
        }

        var useRecaptcha = await _featureManager.IsEnabledAsync("RECAPTCHA_USE_GOOGLE_RECAPTCHA");
        var useTurnstile = await _featureManager.IsEnabledAsync("RECAPTCHA_USE_CLOUDFLARE_TURNSTILE");

        if(useRecaptcha)
        {
            _service = GetService<GoogleCaptchaService>();
        }
        else if(useTurnstile)
        {
            _service = GetService<CloudflareTurnstileCaptchaService>();
        }
        else
        {
            throw new InvalidOperationException("No valid captcha service is configured!");
        }

        return _service;
    }

    ICaptchaService GetService<T>() where T : ICaptchaService
    {
        var svc = _captchaServices.FirstOrDefault(x => x is T);

        if(svc == null)
        {
            throw new InvalidOperationException($"Unable to find captcha service of type {typeof(T)}");
        }

        return svc;
    }
}
