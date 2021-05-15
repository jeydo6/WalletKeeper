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
	public class ChangeUserPasswordHandler : IRequestHandler<ChangeUserPasswordCommand, UserDto>
	{
		private readonly IPrincipal _principal;
		private readonly IUsersRepository _usersRepository;
		private readonly ILogger<ChangeUserPasswordHandler> _logger;

		public ChangeUserPasswordHandler(
			IPrincipal principal,
			IUsersRepository usersRepository,
			ILogger<ChangeUserPasswordHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<UserDto> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
		{
			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(request.Dto.OldPassword))
			{
				throw new ValidationException($"{nameof(request.Dto.OldPassword)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(request.Dto.NewPassword))
			{
				throw new ValidationException($"{nameof(request.Dto.NewPassword)} is invalid");
			}

			var userID = _principal.GetUserID();
			var user = await _usersRepository.ChangePasswordAsync(userID, request.Dto.OldPassword, request.Dto.NewPassword);

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
