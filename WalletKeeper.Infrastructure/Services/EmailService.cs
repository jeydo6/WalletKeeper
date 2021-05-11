using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;
using WalletKeeper.Domain.Configs;
using WalletKeeper.Domain.Services;
using WalletKeeper.Domain.Types;

namespace WalletKeeper.Infrastructure.Services
{
	public class EmailService : IEmailService
	{
		private readonly SmtpConfig _config;

		private readonly ILogger<EmailService> _logger;

		public EmailService(
			IOptions<SmtpConfig> configOptions,
			ILogger<EmailService> logger
		)
		{
			_config = configOptions.Value;

			_logger = logger;
		}

		public async Task SendAsync(EmailMessage message)
		{
			var mimeFrom = new MailboxAddress[]
			{
				new MailboxAddress(_config.EmailAddress, _config.EmailName)
			};
			var mimeTo = new MailboxAddress[]
			{
				new MailboxAddress(message.To.Name, message.To.Address)
			};
			var mimeSubject = message.Subject;
			var mimeBody = new TextPart(TextFormat.Html)
			{
				Text = message.Body
			};

			var mimeMessage = new MimeMessage(mimeFrom, mimeTo, mimeSubject, mimeBody);

			_logger.LogDebug($"Start initialization of smtp client");
			using (var client = new SmtpClient())
			{
				_logger.LogDebug($"Start connecting ({_config.Address}, {_config.Port}, {_config.UseSSL})");
				await client.ConnectAsync(_config.Address, _config.Port, _config.UseSSL);
				_logger.LogDebug($"Success connecting ({_config.Address}, {_config.Port}, {_config.UseSSL})");

				_logger.LogDebug($"Start authentication ({_config.UserName}, {_config.Password})");
				await client.AuthenticateAsync(_config.UserName, _config.Password);
				_logger.LogDebug($"Success authentication ({_config.UserName}, {_config.Password})");

				_logger.LogDebug("Start sending a message: @{mimeMessage}", mimeMessage);
				await client.SendAsync(mimeMessage);
				_logger.LogDebug("Success sending a message");

				await client.DisconnectAsync(true);
			}
			_logger.LogDebug($"Success disposing of smtp client");
		}
	}
}
