using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class ChangeUserEmailCommand : IRequest<UserDto>
	{
		public ChangeUserEmailCommand(String token, ChangeUserEmailDto dto)
		{
			Token = token;
			Dto = dto;
		}

		public String Token { get; }

		public ChangeUserEmailDto Dto { get; }
	}
}
