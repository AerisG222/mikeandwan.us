using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;


namespace Maw.Domain.Captcha
{
	//https://www.google.com/recaptcha/admin#site/318682987?setup
	public class GoogleCaptchaService
		: ICaptchaService
	{
		const string URL = "https://www.google.com/recaptcha/api/siteverify";
		readonly GoogleCaptchaConfig _config;
		readonly ILogger _log;


		public GoogleCaptchaService(IOptions<GoogleCaptchaConfig> config, ILoggerFactory loggerFactory)
		{
			if(config == null)
			{
				throw new ArgumentNullException(nameof(config));
			}

			if(loggerFactory == null)
			{
				throw new ArgumentNullException(nameof(loggerFactory));
			}

			_config = config.Value;
			_log = loggerFactory.CreateLogger<GoogleCaptchaService>();
		}


		public virtual string SiteKey 
		{
			get
			{
				return _config.RecaptchaSiteKey;
			}
		}


		public virtual async Task<bool> VerifyAsync(string recaptchaResponse)
		{
			if(string.IsNullOrEmpty(recaptchaResponse))
			{
				return false;
			}

			var parameters = new List<KeyValuePair<string, string>>();

			parameters.Add(new KeyValuePair<string, string>("secret", _config.RecaptchaSecretKey));
			parameters.Add(new KeyValuePair<string, string>("response", recaptchaResponse));
			
			using(var client = new HttpClient())
			using(var content = new FormUrlEncodedContent(parameters))
			{
				var response = await client.PostAsync(URL, content).ConfigureAwait(false);
				var val = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				var jobject = JObject.Parse(val);
				var result = (bool)jobject["success"];

				_log.LogInformation("google capture returned: " + result.ToString());

				response.Dispose();
				
				return result;
			}
		}
	}
}

