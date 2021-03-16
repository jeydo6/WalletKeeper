using System;

namespace WalletKeeper.WebAPI.Configs
{
	public class EndpointConfig
	{
		public String Issuer { get; set; }

		public String[] Audiences { get; set; }

		public String Secret { get; set; }
	}
}
