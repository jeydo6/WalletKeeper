using System;

namespace WalletKeeper.Application.Dto
{
	public class ClaimDto
	{
		public String Type { get; set; }

		public String Value { get; set; }

		public String ValueType { get; set; }

		public String Issuer { get; set; }

		public String OriginalIssuer { get; set; }
	}
}
