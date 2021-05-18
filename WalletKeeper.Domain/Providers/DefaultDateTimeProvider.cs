using System;

namespace WalletKeeper.Domain.Providers
{
	public class DefaultDateTimeProvider : IDateTimeProvider
	{
		public DateTime Now => new(2021, 6, 1, 12, 0, 0);

		public DateTime UtcNow => new(2021, 6, 1, 9, 0, 0);
	}
}
