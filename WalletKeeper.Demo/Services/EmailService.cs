using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WalletKeeper.Domain.Services;
using WalletKeeper.Domain.Types;

namespace WalletKeeper.Demo.Services
{
	public class EmailService : IEmailService
	{
		private readonly ILogger<EmailService> _logger;

		public EmailService(
			ILogger<EmailService> logger
		)
		{
			_logger = logger;
		}

		public async Task SendAsync(EmailMessage emailMessage)
		{
			await Task.CompletedTask;
		}
	}
}
