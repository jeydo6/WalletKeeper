using System;
using System.Collections.Generic;

namespace WalletKeeper.Domain.Types
{
	public class Product
	{
		public Product()
		{
			ProductItems = new List<ProductItem>();
		}

		public Int32 ID { get; set; }

		public String Name { get; set; }

		public Category Category { get; set; }

		public List<ProductItem> ProductItems { get; set; }
	}
}
