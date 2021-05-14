using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletKeeper.Domain.Entities
{
	[Table("Products")]
	public class Product
	{
		public Product()
		{
			ProductItems = new List<ProductItem>();
		}

		[Key]
		public Int32 ID { get; set; }

		public String Name { get; set; }

		public Int32? CategoryID { get; set; }

		public Category Category { get; set; }

		public List<ProductItem> ProductItems { get; set; }
	}
}
