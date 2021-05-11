using System;

namespace WalletKeeper.Domain.Types
{
	public class EmailMessage
	{
		public EmailMessage(EmailAddress to, String subject, String body)
		{
			To = to;
			Subject = subject;
			Body = body;
		}

		public EmailAddress To { get; }

		public String Subject { get; }

		public String Body { get; }
	}
}
