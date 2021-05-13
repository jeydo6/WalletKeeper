using MediatR;
using WalletKeeper.Application.Dto;

namespace WalletKeeper.Application.Commands
{
	public class CreateUserCommand : IRequest<UserDto>
	{
		public CreateUserCommand(CreateUserDto dto)
		{
			Dto = dto;
		}

		public CreateUserDto Dto { get; }
	}
}
