namespace Maw.Domain.Email
{
    public class SmtpEmailConfig
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
