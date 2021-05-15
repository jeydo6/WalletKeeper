using System;

namespace WalletKeeper.Domain.Entities
{
	public class ProductItem
	{
		public Int32 ID { get; set; }

		public String Name { get; set; }

		public Decimal Price { get; set; }

		public Decimal Quantity { get; set; }

		public Decimal Sum { get; set; }

		public Int32? ReceiptID { get; set; }

		public Receipt Receipt { get; set; }

		public Int32? ProductID { get; set; }

		public Product Product { get; set; }
	}
}
