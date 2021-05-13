using MediatR;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class ChangeUserPasswordCommand : IRequest<UserDto>
	{
		public ChangeUserPasswordCommand(ChangeUserPasswordDto dto)
		{
			Dto = dto;
		}

		public ChangeUserPasswordDto Dto { get; }
	}
}
