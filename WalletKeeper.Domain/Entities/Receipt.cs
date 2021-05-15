using System;
using System.Collections.Generic;

namespace WalletKeeper.Domain.Entities
{
	public class Receipt
	{
		public Receipt()
		{
			ProductItems = new List<ProductItem>();
		}

		public Int32 ID { get; set; }

		public String FiscalDocumentNumber { get; set; }

		public String FiscalDriveNumber { get; set; }

		public String FiscalType { get; set; }

		public DateTime DateTime { get; set; }

		public Decimal TotalSum { get; set; }

		public Int32 OperationType { get; set; }

		public String Place { get; set; }

		public Int32 OrganizationID { get; set; }

		public Organization Organization { get; set; }

		public List<ProductItem> ProductItems { get; set; }

		public Guid UserID { get; set; }

		public User User { get; set; }
	}
}
