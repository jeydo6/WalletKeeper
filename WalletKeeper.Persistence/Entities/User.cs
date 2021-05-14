using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WalletKeeper.Persistence.Entities
{
	public class User : IdentityUser<Guid>
	{
		public User() : base()
		{
			Receipts = new List<Receipt>();
		}

		public User(String userName) : base(userName)
		{
			Receipts = new List<Receipt>();
		}

		public List<Receipt> Receipts { get; set; }
	}
}
