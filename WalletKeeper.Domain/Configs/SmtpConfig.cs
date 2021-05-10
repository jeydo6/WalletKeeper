using System;

namespace WalletKeeper.Domain.Configs
{
	public class SmtpConfig
	{
		public String Address { get; set; }

		public Int32 Port { get; set; }

		public Boolean UseSSL { get; set; }

		public String UserName { get; set; }

		public String Password { get; set; }
		
		public String EmailAddress { get; set; }

		public String EmailName { get; set; }
	}
}
