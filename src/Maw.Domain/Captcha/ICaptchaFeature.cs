namespace Maw.Domain.Captcha;

public interface ICaptchaFeature
{
    Task<ICaptchaService> GetServiceAsync();
}
