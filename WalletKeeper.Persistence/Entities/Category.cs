using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletKeeper.Persistence.Entities
{
	[Table("Categories")]
	public class Category
	{
		public Category()
		{
			Products = new List<Product>();
		}

		[Key]
		public Int32 ID { get; set; }

		public String Name { get; set; }

		public List<Product> Products { get; set; }
	}
}
