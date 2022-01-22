namespace Maw.Domain.Email;

public class SmtpEmailConfig
{
    public string Server { get; set; } = null!;
    public int Port { get; set; }
    public string User { get; set; } = null!;
    public string Password { get; set; } = null!;
}
