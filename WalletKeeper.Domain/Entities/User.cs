using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WalletKeeper.Domain.Entities
{
	public class User : IdentityUser<Guid>
	{
		public User() : base()
		{
			Products = new List<Product>();
			Receipts = new List<Receipt>();
			ProductItems = new List<ProductItem>();
		}

		public User(String userName) : base(userName)
		{
			Products = new List<Product>();
			Receipts = new List<Receipt>();
			ProductItems = new List<ProductItem>();
		}

		public List<Product> Products { get; set; }

		public List<Receipt> Receipts { get; set; }

		public List<ProductItem> ProductItems { get; set; }
	}
}
