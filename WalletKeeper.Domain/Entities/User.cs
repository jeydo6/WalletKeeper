using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WalletKeeper.Domain.Entities
{
	public class User : IdentityUser<Guid>
	{
		public User() : base()
		{
			Receipts = new List<Receipt>();
		}

		public User(String userName) : base(userName)
		{
			Products = new List<Product>();
			Receipts = new List<Receipt>();
		}

		public List<Product> Products { get; set; }

		public List<Receipt> Receipts { get; set; }
	}
}
