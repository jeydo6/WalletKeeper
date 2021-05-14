using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Application.Extensions;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;

namespace WalletKeeper.Application.Commands
{
	public class ResetUserPasswordHandler : IRequestHandler<ResetUserPasswordCommand, UserDto>
	{
		private readonly UserManager<User> _userManager;

		private readonly IPrincipal _principal;
		private readonly ILogger<ResetUserPasswordHandler> _logger;

		public ResetUserPasswordHandler(
			UserManager<User> userManager,
			IPrincipal principal,
			ILogger<ResetUserPasswordHandler> logger
		)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<UserDto> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
		{
			if (String.IsNullOrWhiteSpace(request.Token))
			{
				throw new ValidationException($"{nameof(request.Token)} is invalid");
			}

			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(request.Dto.Password))
			{
				throw new ValidationException($"{nameof(request.Dto.Password)} is invalid");
			}

			var userID = _principal.GetUserID();
			var user = await _userManager.FindByIdAsync(userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var identityResult = await _userManager.ResetPasswordAsync(user, request.Token, request.Dto.Password);
			identityResult.EnsureSuccess("An error occurred while patching a user", _logger);

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
