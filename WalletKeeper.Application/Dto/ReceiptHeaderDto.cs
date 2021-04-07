using System;

namespace WalletKeeper.Application.Dto
{
	public class ReceiptHeaderDto
	{
		public String FiscalDocumentNumber { get; set; }

		public String FiscalDriveNumber { get; set; }

		public String FiscalType { get; set; }

		public DateTime DateTime { get; set; }

		public Decimal TotalSum { get; set; }

		public Int32 OperationType { get; set; }
	}
}
