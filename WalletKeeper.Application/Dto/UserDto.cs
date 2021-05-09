using System;

namespace WalletKeeper.Application.Dto
{
	public class UserDto
	{
		public Guid ID { get; set; }

		public String UserName { get; set; }

		public String Email { get; set; }

		public Boolean EmailConfirmed { get; set; }
	}
}
