using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace WalletKeeper.Domain.Entities
{
	[Table("Receipts")]
	[Index(nameof(FiscalDocumentNumber), IsUnique = true)]
	public class Receipt
	{
		[Key]
		public Int32 ID { get; set; }

		public String FiscalDocumentNumber { get; set; }

		public String FiscalDriveNumber { get; set; }

		public String FiscalType { get; set; }

		public DateTime DateTime { get; set; }

		public Decimal TotalSum { get; set; }

		public Int32 OperationType { get; set; }

		public static Receipt Parse(String s)
		{
			try
			{
				String fiscalDocumentNumber = default;
				String fiscalDriveNumber = default;
				String fiscalType = default;
				DateTime dateTime = default;
				Decimal totalSum = default;
				Int32 operationType = default;

				foreach (var sub in s.Split('&', StringSplitOptions.RemoveEmptyEntries))
				{
					if (sub.StartsWith("i="))
					{
						fiscalDocumentNumber = sub[2..];
					}
					else if (sub.StartsWith("fn="))
					{
						fiscalDriveNumber = sub[3..];
					}
					else if (sub.StartsWith("fp="))
					{
						fiscalType = sub[3..];
					}
					else if (sub.StartsWith("t="))
					{
						dateTime = DateTime.ParseExact(sub[2..], "yyyyMMddTHHmm", CultureInfo.InvariantCulture);
					}
					else if (sub.StartsWith("s="))
					{
						totalSum = Decimal.Parse(sub[2..], CultureInfo.InvariantCulture);
					}
					else if (sub.StartsWith("n="))
					{
						operationType = Int32.Parse(sub[2..]);
					}
				}

				return new Receipt
				{
					FiscalDocumentNumber = fiscalDocumentNumber,
					FiscalDriveNumber = fiscalDriveNumber,
					FiscalType = fiscalType,
					DateTime = dateTime,
					TotalSum = totalSum,
					OperationType = operationType
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
		}
	}
}
