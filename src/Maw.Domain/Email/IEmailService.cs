using System.Threading.Tasks;


namespace Maw.Domain.Email
{
	public interface IEmailService
	{
		Task SendAsync(string to, string from, string subject, string body);
        Task SendHtmlAsync(string to, string from, string subject, string body);
	}
}
