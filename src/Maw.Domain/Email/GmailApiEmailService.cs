using System;
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
        static string[] Scopes = { GmailService.Scope.GmailSend };

        readonly GmailApiEmailConfig _config;
        readonly ILogger _log;


        public string FromAddress => _config.FromEmailAddress;


        public GmailApiEmailService(ILogger<GmailApiEmailService> log, IOptions<GmailApiEmailConfig> config)
		{
			if(config == null)
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
            _log.LogInformation($"sending email to: {recipient}, from: {from}, subject: {subject}");

            var msg = BuildMessage(recipient, from, subject, body, isHtml);

            using(var svc = await GetService().ConfigureAwait(false))
            {
                var req = svc.Users.Messages.Send(msg, "me");

                var result = await req.ExecuteAsync().ConfigureAwait(false);
            }
        }


        static Message BuildMessage(string recipient, string from, string subject, string body, bool isHtml)
        {
            var builder = new BodyBuilder();

            if(isHtml)
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

            return new Message {
                Raw = Encode(msg.ToString())
            };
        }


        static string Encode(string text)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);

            string s = Convert.ToBase64String(bytes); // Standard base64 encoder

            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding

            return s;
        }


        async Task<GmailService> GetService() {
            var cred = await GoogleCredential.GetApplicationDefaultAsync().ConfigureAwait(false);

            cred = cred.CreateScoped(Scopes)
                       .CreateWithUser(_config.FromEmailAddress);

            var svc = new GmailService(new BaseClientService.Initializer() {
                HttpClientInitializer = cred,
                ApplicationName = _config.ApplicationName
            });

            return svc;
        }
    }
}
