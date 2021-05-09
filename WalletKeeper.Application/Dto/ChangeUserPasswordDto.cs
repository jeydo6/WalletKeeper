using System;

namespace WalletKeeper.Application.Dto
{
	public class ChangeUserPasswordDto
	{
		public String OldPassword { get; set; }

		public String NewPassword { get; set; }
	}
}
