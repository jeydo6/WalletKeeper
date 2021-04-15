using System;

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

		public String DateTime { get; private set; }

		public String TotalSum { get; private set; }

		public String OperationType { get; private set; }

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

			var qrcode = new QRCode();

			foreach (var item in qrcodeString.Split('&', StringSplitOptions.RemoveEmptyEntries))
			{
				if (item.StartsWith("i="))
				{
					qrcode.FiscalDocumentNumber = item[2..];
				}
				else if (item.StartsWith("fn="))
				{
					qrcode.FiscalDriveNumber = item[3..];
				}
				else if (item.StartsWith("fp="))
				{
					qrcode.FiscalType = item[3..];
				}
				else if (item.StartsWith("t="))
				{
					qrcode.DateTime = item[2..];
				}
				else if (item.StartsWith("s="))
				{
					qrcode.TotalSum = item[2..];
				}
				else if (item.StartsWith("n="))
				{
					qrcode.OperationType = item[2..];
				}
			}

			return qrcode;
		}
	}
}
