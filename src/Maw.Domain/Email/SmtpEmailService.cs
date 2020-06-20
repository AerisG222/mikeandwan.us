using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace Maw.Domain.Email
{
    public class SmtpEmailService
        : IEmailService
    {
        readonly SmtpEmailConfig _config;
        readonly ILogger _log;


        public string FromAddress => _config.User;


        public SmtpEmailService(ILogger<SmtpEmailService> log, IOptions<SmtpEmailConfig> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _config = config.Value;
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }


        public virtual Task SendHtmlAsync(string recipient, string from, string subject, string body)
        {
            return SendAsync(recipient, from, subject, body, true);
        }


        public virtual Task SendAsync(string recipient, string from, string subject, string body)
        {
            return SendAsync(recipient, from, subject, body, false);
        }


        protected virtual async Task SendAsync(string recipient, string from, string subject, string body, bool html)
        {
            _log.LogInformation("sending email to: {Recipient}, from: {From}, subject: {Subject}", recipient, from, subject);

            using var smtp = new SmtpClient();
            var builder = new BodyBuilder();

            if (html)
            {
                builder.HtmlBody = body;
            }
            else
            {
                builder.TextBody = body;
            }

            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress((string)null, from));
            msg.To.Add(new MailboxAddress((string)null, recipient));
            msg.Subject = subject;
            msg.Body = builder.ToMessageBody();

            // http://stackoverflow.com/questions/33496290/how-to-send-email-by-using-mailkit
            await smtp.ConnectAsync(_config.Server, _config.Port, SecureSocketOptions.StartTls).ConfigureAwait(false);
            await smtp.AuthenticateAsync(_config.User, _config.Password).ConfigureAwait(false);
            await smtp.SendAsync(msg).ConfigureAwait(false);
            await smtp.DisconnectAsync(true).ConfigureAwait(false);
        }
    }
}

