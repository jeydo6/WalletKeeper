using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Application.Extensions;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Factories;
using WalletKeeper.Domain.Services;
using WalletKeeper.Domain.Types;
using WalletKeeper.Persistence.Entities;

namespace WalletKeeper.Application.Queries
{
	public class ResetUserPasswordHandler : IRequestHandler<ResetUserPasswordQuery>
	{
		private readonly UserManager<User> _userManager;
		private readonly EmailMessageFactory _emailMessageFactory;

		private readonly IPrincipal _principal;
		private readonly IEmailService _emailService;
		private readonly ILogger<ResetUserPasswordHandler> _logger;

		public ResetUserPasswordHandler(
			UserManager<User> userManager,
			EmailMessageFactory emailMessageFactory,
			IPrincipal principal,
			IEmailService emailService,
			ILogger<ResetUserPasswordHandler> logger
		)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_emailMessageFactory = emailMessageFactory ?? throw new ArgumentNullException(nameof(emailMessageFactory));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Unit> Handle(ResetUserPasswordQuery request, CancellationToken cancellationToken)
		{
			var userID = _principal.GetUserID();
			var user = await _userManager.FindByIdAsync(userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var to = new EmailAddress(user.Email, user.UserName);
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);

			var emailMessage = _emailMessageFactory.CreatePasswordResettingMessage(to, token);

			await _emailService.SendAsync(emailMessage);

			return Unit.Value;
		}
	}
}
