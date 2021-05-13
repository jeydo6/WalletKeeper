using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class ResetUserPasswordCommand : IRequest<UserDto>
	{
		public ResetUserPasswordCommand(String token, ResetUserPasswordDto dto)
		{
			Token = token;
			Dto = dto;
		}

		public String Token { get; }

		public ResetUserPasswordDto Dto { get; }
	}
}
