using MediatR;
using System;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class ConfirmUserEmailCommand : IRequest<UserDto>
	{
		public ConfirmUserEmailCommand(String token)
		{
			Token = token;
		}

		public String Token { get; }
	}
}
