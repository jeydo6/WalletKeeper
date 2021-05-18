using System;

namespace WalletKeeper.Domain.Providers
{
	public interface IDateTimeProvider
	{
		DateTime Now { get; }

		DateTime UtcNow { get; }
	}
}
