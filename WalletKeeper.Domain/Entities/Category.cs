using System;
using System.Collections.Generic;

namespace WalletKeeper.Domain.Entities
{
	public class Category
	{
		public Category()
		{
			Products = new List<Product>();
		}

		public Int32 ID { get; set; }

		public String Name { get; set; }

		public List<Product> Products { get; set; }
	}
}
