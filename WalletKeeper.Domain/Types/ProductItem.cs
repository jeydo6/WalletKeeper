using System;

namespace WalletKeeper.Domain.Types
{
	public class ProductItem
	{
		public Int32 ID { get; set; }

		public String Name { get; set; }

		public Decimal Price { get; set; }

		public Decimal Quantity { get; set; }

		public Decimal Sum { get; set; }

		public Receipt Receipt { get; set; }

		public Product Product { get; set; }
	}
}
