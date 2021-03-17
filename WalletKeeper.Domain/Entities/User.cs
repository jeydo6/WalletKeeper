using Microsoft.AspNetCore.Identity;
using System;

namespace WalletKeeper.Domain.Entities
{
	public class User : IdentityUser<Guid>
	{
		public User()
			: base()
		{
			//
		}

		public User(String userName)
			: base(userName)
		{
			//
		}
	}
}
