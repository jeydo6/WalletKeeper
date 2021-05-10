using System;

namespace WalletKeeper.Domain.Types
{
	public class EmailMessage
	{
		public EmailMessage(EmailAddress from, EmailAddress to, String subject, String body)
		{
			From = from;
			To = to;
			Subject = subject;
			Body = body;
		}

		public EmailAddress From { get; }

		public EmailAddress To { get; }

		public String Subject { get; }

		public String Body { get; }
	}
}
