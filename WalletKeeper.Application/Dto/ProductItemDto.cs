using System;

namespace WalletKeeper.Application.Dto
{
	public class ProductItemDto
	{
		public Int32 ID { get; set; }

		public String Name { get; set; }

		public Decimal Price { get; set; }

		public Decimal Quantity { get; set; }

		public Decimal Sum { get; set; }

		public Decimal NDS { get; set; }

		public Int32? ReceiptID { get; set; }

		public Int32? ProductID { get; set; }
	}
}
