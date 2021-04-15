using System;

namespace WalletKeeper.Domain.Configs
{
	public class AuthenticationConfig
	{
		public String Issuer { get; set; }

		public String[] Audiences { get; set; }

		public String Secret { get; set; }
	}
}
