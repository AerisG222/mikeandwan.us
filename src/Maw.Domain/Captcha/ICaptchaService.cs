namespace Maw.Domain.Captcha;

public interface ICaptchaService
{
    string SiteKey { get; }
    Task<bool> VerifyAsync(string recaptchaResponse);
}
