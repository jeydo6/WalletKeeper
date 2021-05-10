using System;

namespace WalletKeeper.Domain.Types
{
	public class EmailAddress
	{
		public EmailAddress(String address, String name = null)
		{
			Address = address;
			Name = name;
		}

		public String Name { get; }

		public String Address { get; }
	}
}
