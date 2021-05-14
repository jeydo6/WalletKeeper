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
	public class ChangeUserNameHandler : IRequestHandler<ChangeUserNameCommand, UserDto>
	{
		private readonly UserManager<User> _userManager;

		private readonly IPrincipal _principal;
		private readonly ILogger<ChangeUserNameHandler> _logger;

		public ChangeUserNameHandler(
			UserManager<User> userManager,
			IPrincipal principal,
			ILogger<ChangeUserNameHandler> logger
		)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<UserDto> Handle(ChangeUserNameCommand request, CancellationToken cancellationToken)
		{
			if (request.Dto == null)
			{
				throw new ValidationException($"{nameof(request.Dto)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(request.Dto.UserName))
			{
				throw new ValidationException($"{nameof(request.Dto.UserName)} is invalid");
			}

			var userID = _principal.GetUserID();
			var user = await _userManager.FindByIdAsync(userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var identityResult = await _userManager.SetUserNameAsync(user, request.Dto.UserName);
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
