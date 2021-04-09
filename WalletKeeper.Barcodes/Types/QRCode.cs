using System;
using System.Globalization;

namespace WalletKeeper.Barcodes.Types
{
	public class QRCode
	{
		private QRCode()
		{
			//
		}

		public String FiscalDocumentNumber { get; private set; }

		public String FiscalDriveNumber { get; private set; }

		public String FiscalType { get; private set; }

		public DateTime DateTime { get; private set; }

		public Decimal TotalSum { get; private set; }

		public Int32 OperationType { get; private set; }

		public static QRCode Parse(String qrcodeString)
		{
			if (String.IsNullOrEmpty(qrcodeString))
			{
				throw new ArgumentNullException(qrcodeString);
			}

			if (!qrcodeString.Contains("&"))
			{
				throw new ArgumentException(qrcodeString);
			}

			String fiscalDocumentNumber = default;
			String fiscalDriveNumber = default;
			String fiscalType = default;
			DateTime dateTime = default;
			Decimal totalSum = default;
			Int32 operationType = default;

			foreach (var item in qrcodeString.Split('&', StringSplitOptions.RemoveEmptyEntries))
			{
				if (item.StartsWith("i="))
				{
					fiscalDocumentNumber = item[2..];
				}
				else if (item.StartsWith("fn="))
				{
					fiscalDriveNumber = item[3..];
				}
				else if (item.StartsWith("fp="))
				{
					fiscalType = item[3..];
				}
				else if (item.StartsWith("t="))
				{
					dateTime = DateTime.ParseExact(item[2..], "yyyyMMddTHHmm", CultureInfo.InvariantCulture);
				}
				else if (item.StartsWith("s="))
				{
					totalSum = Decimal.Parse(item[2..], CultureInfo.InvariantCulture);
				}
				else if (item.StartsWith("n="))
				{
					operationType = Int32.Parse(item[2..]);
				}
			}

			return new QRCode
			{
				FiscalDocumentNumber = fiscalDocumentNumber,
				FiscalDriveNumber = fiscalDriveNumber,
				FiscalType = fiscalType,
				DateTime = dateTime,
				TotalSum = totalSum,
				OperationType = operationType
			};
		}
	}
}
