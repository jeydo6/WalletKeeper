using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletKeeper.Persistence.Entities
{
	[Table("Receipts")]
	[Index(nameof(FiscalDocumentNumber), IsUnique = true)]
	public class Receipt
	{
		public Receipt()
		{
			ProductItems = new List<ProductItem>();
		}

		[Key]
		public Int32 ID { get; set; }

		public String FiscalDocumentNumber { get; set; }

		public String FiscalDriveNumber { get; set; }

		public String FiscalType { get; set; }

		public DateTime DateTime { get; set; }

		[Column(TypeName = "decimal(18, 2)")]
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
