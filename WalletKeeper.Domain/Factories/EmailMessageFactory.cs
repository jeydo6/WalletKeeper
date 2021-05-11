using Microsoft.Extensions.Logging;
using System;
using WalletKeeper.Domain.Types;

namespace WalletKeeper.Domain.Factories
{
	public class EmailMessageFactory
	{
		private readonly ILogger<EmailMessageFactory> _logger;

		public EmailMessageFactory(
			ILogger<EmailMessageFactory> logger
		)
		{
			_logger = logger;
		}

		public EmailMessage CreateEmailConfirmationMessage(EmailAddress to, String token)
		{
			_logger.LogDebug($"Start creating an {nameof(EmailMessage)}");

			return new EmailMessage(
				to: to,
				subject: "Email address confirmation",
				body: $"Email address confirmation <b>token</b>: {Uri.EscapeDataString(token)}"
			);
		}

		public EmailMessage CreateEmailChangingMessage(EmailAddress to, String token)
		{
			_logger.LogDebug($"Start creating an {nameof(EmailMessage)}");

			return new EmailMessage(
				to: to,
				subject: "Email address changing",
				body: $"Email address changing <b>token</b>: {Uri.EscapeDataString(token)}"
			);
		}

		public EmailMessage CreatePasswordResettingMessage(EmailAddress to, String token)
		{
			_logger.LogDebug($"Start creating an {nameof(EmailMessage)}");

			return new EmailMessage(
				to: to,
				subject: "Password resetting",
				body: $"Password resetting <b>token</b>: {Uri.EscapeDataString(token)}"
			);
		}
	}
}
