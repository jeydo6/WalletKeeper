using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalletKeeper.Persistence.Entities
{
	[Table("Organizations")]
	[Index(nameof(INN), IsUnique = true)]
	public class Organization
	{
		[Key]
		public Int32 ID { get; set; }

		public String INN { get; set; }

		public String Name { get; set; }
	}
}
