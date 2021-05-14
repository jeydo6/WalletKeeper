using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Application.Extensions;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;

namespace WalletKeeper.Application.Commands
{
	public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
	{
		private readonly UserManager<User> _userManager;

		private readonly ILogger<CreateUserHandler> _logger;

		public CreateUserHandler(
			UserManager<User> userManager,
			ILogger<CreateUserHandler> logger
		)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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

			var identityResult = await _userManager.CreateAsync(user, request.Dto.Password);
			identityResult.EnsureSuccess("An error occurred while creating a user", _logger);

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
