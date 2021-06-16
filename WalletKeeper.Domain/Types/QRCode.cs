using System;

namespace WalletKeeper.Domain.Types
{
	public class QRCode
	{
		public String FiscalDocumentNumber { get; set; }

		public String FiscalDriveNumber { get; set; }

		public String FiscalType { get; set; }

		public String DateTime { get; set; }

		public String TotalSum { get; set; }

		public String OperationType { get; set; }

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

		public override String ToString()
		{
			return $"t={DateTime}&s={TotalSum}&fn={FiscalDriveNumber}&i={FiscalDocumentNumber}&fp={FiscalType}&n={OperationType}";
		}
	}
}
