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
	public class ConfirmUserEmailHandler : IRequestHandler<ConfirmUserEmailCommand, UserDto>
	{
		private readonly IPrincipal _principal;
		private readonly IUsersRepository _usersRepository;
		private readonly ILogger<ConfirmUserEmailHandler> _logger;

		public ConfirmUserEmailHandler(
			IPrincipal principal,
			IUsersRepository usersRepository,
			ILogger<ConfirmUserEmailHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<UserDto> Handle(ConfirmUserEmailCommand request, CancellationToken cancellationToken)
		{
			if (String.IsNullOrWhiteSpace(request.Token))
			{
				throw new ValidationException($"{nameof(request.Token)} is invalid");
			}

			var userID = _principal.GetUserID();
			var user = await _usersRepository.ConfirmEmailAsync(userID, request.Token);

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
