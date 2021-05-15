using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Commands
{
	public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
	{
		private readonly IUsersRepository _usersRepository;
		private readonly ILogger<CreateUserHandler> _logger;

		public CreateUserHandler(
			IUsersRepository usersRepository,
			ILogger<CreateUserHandler> logger
		)
		{
			_usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
		{
			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(request.Dto.UserName))
			{
				throw new ValidationException($"{nameof(request.Dto.UserName)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(request.Dto.Password))
			{
				throw new ValidationException($"{nameof(request.Dto.Password)} is invalid");
			}

			var user = new User
			{
				UserName = request.Dto.UserName,
				Email = request.Dto.Email,
				EmailConfirmed = false
			};

			user = await _usersRepository.CreateAsync(user, request.Dto.Password);

			var result = new UserDto
			{
				ID = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				EmailConfirmed = user.EmailConfirmed
			};

			return result;

		}
	}
}
