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

		public EmailAddress From { get; private set; }

		public EmailAddress To { get; private set; }

		public String Subject { get; private set; }

		public String Body { get; private set; }
	}
}
