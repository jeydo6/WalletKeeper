using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Extensions;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;

namespace WalletKeeper.Application.Commands
{
	public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
	{
		private readonly UserManager<User> _userManager;

		private readonly IPrincipal _principal;
		private readonly ILogger<DeleteUserHandler> _logger;

		public DeleteUserHandler(
			UserManager<User> userManager,
			IPrincipal principal,
			ILogger<DeleteUserHandler> logger
		)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
		{
			var userID = _principal.GetUserID();
			var user = await _userManager.FindByIdAsync(userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var identityResult = await _userManager.DeleteAsync(user);
			identityResult.EnsureSuccess("An error occurred while deleting a user", _logger);

			return Unit.Value;
		}
	}
}
