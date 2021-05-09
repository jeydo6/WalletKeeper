using System;

namespace WalletKeeper.Application.Dto
{
	public class CreateUserDto
	{
		public String UserName { get; set; }

		public String Password { get; set; }

		public String Email { get; set; }
	}
}
