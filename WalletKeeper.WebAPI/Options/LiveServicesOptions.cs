using System;
using WalletKeeper.Domain.Configs;

namespace WalletKeeper.WebAPI.Options
{
	internal class LiveServicesOptions
	{
		public String ConnectionString { get; set; }

		public AuthenticationConfig AuthenticationConfig { get; set; }
	}
}
