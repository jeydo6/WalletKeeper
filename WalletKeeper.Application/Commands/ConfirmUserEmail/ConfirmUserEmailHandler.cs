using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Application.Extensions;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Persistence.Entities;

namespace WalletKeeper.Application.Commands
{
	public class ConfirmUserEmailHandler : IRequestHandler<ConfirmUserEmailCommand, UserDto>
	{
		private readonly UserManager<User> _userManager;

		private readonly IPrincipal _principal;
		private readonly ILogger<ConfirmUserEmailHandler> _logger;

		public ConfirmUserEmailHandler(
			UserManager<User> userManager,
			IPrincipal principal,
			ILogger<ConfirmUserEmailHandler> logger
		)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<UserDto> Handle(ConfirmUserEmailCommand request, CancellationToken cancellationToken)
		{
			if (String.IsNullOrWhiteSpace(request.Token))
			{
				throw new ValidationException($"{nameof(request.Token)} is invalid");
			}

			var userID = _principal.GetUserID();
			var user = await _userManager.FindByIdAsync(userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var identityResult = await _userManager.ConfirmEmailAsync(user, request.Token);
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
