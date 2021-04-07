using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletKeeper.Domain.Entities
{
	[Table("Receipts")]
	[Index(nameof(FiscalDocumentNumber), IsUnique = true)]
	public class Receipt : ReceiptHeader
	{
		public Receipt()
		{
			Products = new List<Product>();
		}

		public String Place { get; set; }

		public Int32 OrganizationID { get; set; }

		public Organization Organization { get; set; }

		public List<Product> Products { get; set; }
	}
}
