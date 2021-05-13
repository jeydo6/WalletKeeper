using MediatR;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class ChangeUserNameCommand : IRequest<UserDto>
	{
		public ChangeUserNameCommand(ChangeUserNameDto dto)
		{
			Dto = dto;
		}

		public ChangeUserNameDto Dto { get; }
	}
}
