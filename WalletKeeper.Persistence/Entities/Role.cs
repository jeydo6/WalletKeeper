using Microsoft.AspNetCore.Identity;
using System;

namespace WalletKeeper.Persistence.Entities
{
	public class Role : IdentityRole<Guid>
	{
		public Role() : base()
		{
			//
		}

		public Role(String roleName) : base(roleName)
		{
			//
		}

		public Int32 Level { get; set; }
	}
}
