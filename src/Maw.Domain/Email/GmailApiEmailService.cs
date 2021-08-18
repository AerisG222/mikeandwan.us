using System;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;


namespace Maw.Domain.Email
{
    // https://stackoverflow.com/questions/24728793/creating-a-message-for-gmail-api-in-c-sharp
    // https://github.com/IdentityModel/Thinktecture.IdentityModel/blob/e56dd7699ac5e59df51b6724cff2cda2f2ca10ca/source/Client.Shared/Base64Url.cs
    public class GmailApiEmailService
        : IEmailService
    {
        static readonly string[] Scopes = { GmailService.Scope.GmailSend };

        readonly GmailApiEmailConfig _config;
        readonly ILogger _log;


        public string FromAddress => _config.FromEmailAddress;


        public GmailApiEmailService(ILogger<GmailApiEmailService> log, IOptions<GmailApiEmailConfig> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _config = config.Value;
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }


        public Task SendAsync(string recipient, string from, string subject, string body)
        {
            return SendMessageAsync(recipient, from, subject, body, false);
        }


        public Task SendHtmlAsync(string recipient, string from, string subject, string body)
        {
            return SendMessageAsync(recipient, from, subject, body, true);
        }


        async Task SendMessageAsync(string recipient, string from, string subject, string body, bool isHtml)
        {
            _log.LogInformation("sending email to: {Recipient}, from: {From}, subject: {Subject}", recipient, from, subject);

            var msg = BuildMessage(recipient, from, subject, body, isHtml);

            using var svc = await GetService().ConfigureAwait(false);

            var req = svc.Users.Messages.Send(msg, "me");

            var result = await req.ExecuteAsync().ConfigureAwait(false);
        }


        static Message BuildMessage(string recipient, string from, string subject, string body, bool isHtml)
        {
            var builder = new BodyBuilder();

            if (isHtml)
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

            return new Message
            {
                Raw = Base64UrlEncode(msg)
            };
        }

        // https://stackoverflow.com/questions/35655019/gmail-draft-html-with-attachment-with-mimekit-c-sharp-winforms-and-google-api
        static string Base64UrlEncode (MimeMessage message)
        {
            using (var stream = new MemoryStream ()) {
                message.WriteTo(stream);

                return Convert.ToBase64String(stream.GetBuffer(), 0, (int) stream.Length)
                    .Replace ('+', '-')
                    .Replace ('/', '_')
                    .Replace ("=", "", StringComparison.Ordinal);
            }
        }

        async Task<GmailService> GetService()
        {
            var cred = await GoogleCredential.GetApplicationDefaultAsync().ConfigureAwait(false);

            cred = cred.CreateScoped(Scopes)
                       .CreateWithUser(_config.FromEmailAddress);

            var svc = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred,
                ApplicationName = _config.ApplicationName
            });

            return svc;
        }
    }
}
