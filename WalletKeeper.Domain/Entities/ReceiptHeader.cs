using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletKeeper.Domain.Entities
{
	[Table("ReceiptHeaders")]
	[Index(nameof(FiscalDocumentNumber), IsUnique = true)]
	public class ReceiptHeader
	{
		[Key]
		public Int32 ID { get; set; }

		public String FiscalDocumentNumber { get; set; }

		public String FiscalDriveNumber { get; set; }

		public String FiscalType { get; set; }

		public DateTime DateTime { get; set; }

		public Decimal TotalSum { get; set; }

		public Int32 OperationType { get; set; }
	}
}
