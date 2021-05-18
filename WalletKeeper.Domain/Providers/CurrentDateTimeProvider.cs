using System;

namespace WalletKeeper.Domain.Providers
{
	public class CurrentDateTimeProvider : IDateTimeProvider
	{
		public DateTime Now => DateTime.Now;

		public DateTime UtcNow => DateTime.UtcNow;
	}
}