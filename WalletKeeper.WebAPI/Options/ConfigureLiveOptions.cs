using System;
using WalletKeeper.Domain.Configs;

namespace WalletKeeper.WebAPI.Options
{
	internal class ConfigureLiveOptions
	{
		public String ConnectionString { get; set; }

		public AuthenticationConfig AuthenticationConfig { get; set; }
	}
}
