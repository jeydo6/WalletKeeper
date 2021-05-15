using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Extensions;
using WalletKeeper.Domain.Factories;
using WalletKeeper.Domain.Repositories;
using WalletKeeper.Domain.Services;
using WalletKeeper.Domain.Types;

namespace WalletKeeper.Application.Queries
{
	public class ResetUserPasswordHandler : IRequestHandler<ResetUserPasswordQuery>
	{
		private readonly EmailMessageFactory _emailMessageFactory;

		private readonly IPrincipal _principal;
		private readonly IEmailService _emailService;
		private readonly IUsersRepository _usersRepository;
		private readonly ILogger<ResetUserPasswordHandler> _logger;

		public ResetUserPasswordHandler(
			EmailMessageFactory emailMessageFactory,
			IPrincipal principal,
			IEmailService emailService,
			IUsersRepository usersRepository,
			ILogger<ResetUserPasswordHandler> logger
		)
		{
			_emailMessageFactory = emailMessageFactory ?? throw new ArgumentNullException(nameof(emailMessageFactory));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
			_usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Unit> Handle(ResetUserPasswordQuery request, CancellationToken cancellationToken)
		{
			var userID = _principal.GetUserID();
			var user = await _usersRepository.GetAsync(userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var to = new EmailAddress(user.Email, user.UserName);
			var token = await _usersRepository.GeneratePasswordResetTokenAsync(user);

			var emailMessage = _emailMessageFactory.CreatePasswordResettingMessage(to, token);

			await _emailService.SendAsync(emailMessage);

			return Unit.Value;
		}
	}
}
