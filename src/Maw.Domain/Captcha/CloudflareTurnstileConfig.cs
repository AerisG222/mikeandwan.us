namespace Maw.Domain.Captcha;

public class CloudflareTurnstileConfig
{
    public string SiteKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
}
