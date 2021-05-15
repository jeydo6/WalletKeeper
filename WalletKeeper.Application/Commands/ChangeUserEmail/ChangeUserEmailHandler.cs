using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Extensions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Commands
{
	public class ChangeUserEmailHandler : IRequestHandler<ChangeUserEmailCommand, UserDto>
	{
		private readonly IPrincipal _principal;
		private readonly IUsersRepository _usersRepository;
		private readonly ILogger<ChangeUserEmailHandler> _logger;

		public ChangeUserEmailHandler(
			IPrincipal principal,
			IUsersRepository usersRepository,
			ILogger<ChangeUserEmailHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<UserDto> Handle(ChangeUserEmailCommand request, CancellationToken cancellationToken)
		{
			if (String.IsNullOrWhiteSpace(request.Token))
			{
				throw new ValidationException($"{nameof(request.Token)} is invalid");
			}

			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(request.Dto.Email))
			{
				throw new ValidationException($"{nameof(request.Dto.Email)} is invalid");
			}

			var userID = _principal.GetUserID();
			var user = await _usersRepository.ChangeEmailAsync(userID, request.Dto.Email, request.Token);

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
