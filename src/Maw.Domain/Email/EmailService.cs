using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace Maw.Domain.Email
{
	public class EmailService
	    : IEmailService
	{
		readonly EmailConfig _config;
		readonly ILogger _log;


		public EmailService(ILoggerFactory loggerFactory, IOptions<EmailConfig> config)
		{
			if(loggerFactory == null)
			{
				throw new ArgumentNullException(nameof(loggerFactory));
			}

			if(config == null)
			{
				throw new ArgumentNullException(nameof(config));
			}

			_config = config.Value;
			_log = loggerFactory.CreateLogger<EmailService>();
		}


        public virtual Task SendHtmlAsync(string to, string from, string subject, string body)
        {
            return SendAsync(to, from, subject, body, true);
        }
        
        
        public virtual Task SendAsync(string to, string from, string subject, string body)
        {
            return SendAsync(to, from, subject, body, false);
        }
        
        
		protected virtual async Task SendAsync(string to, string from, string subject, string body, bool html)
		{
			_log.LogInformation(string.Format("sending email to: {0}, from: {1}, subject: {2}", to, from, subject));

			using(var smtp = new SmtpClient())
			{
                var builder = new BodyBuilder();
                
                if(html)
                {
                    builder.HtmlBody = body;
                }
                else
                {
                    builder.TextBody = body;
                }
                
                var msg = new MimeMessage();
                msg.From.Add(new MailboxAddress(null, from));
				msg.To.Add(new MailboxAddress(null, to));
				msg.Subject = subject;
				msg.Body = builder.ToMessageBody();
                
                // TODO: look to use razorengine for emails
                // TODO: consider using oauth2/cert and not require this to be configured as a 'less secure app'
                //       http://stackoverflow.com/questions/33496290/how-to-send-email-by-using-mailkit
                await smtp.ConnectAsync(_config.Server, _config.Port, SecureSocketOptions.StartTls).ConfigureAwait(false);
                await smtp.AuthenticateAsync(_config.User, _config.Password).ConfigureAwait(false);
				await smtp.SendAsync(msg).ConfigureAwait(false);
                await smtp.DisconnectAsync(true).ConfigureAwait(false);
			}
		}
	}
}

