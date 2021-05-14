using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletKeeper.Domain.Entities
{
	[Table("ProductItems")]
	public class ProductItem
	{
		[Key]
		public Int32 ID { get; set; }

		public String Name { get; set; }

		[Column(TypeName = "decimal(18, 2)")]
		public Decimal Price { get; set; }

		[Column(TypeName = "decimal(18, 4)")]
		public Decimal Quantity { get; set; }

		[Column(TypeName = "decimal(18, 2)")]
		public Decimal Sum { get; set; }

		public Int32? ReceiptID { get; set; }

		public Receipt Receipt { get; set; }

		public Int32? ProductID { get; set; }

		public Product Product { get; set; }
	}
}
