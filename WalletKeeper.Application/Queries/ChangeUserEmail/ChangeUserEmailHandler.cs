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
	public class ChangeUserEmailHandler : IRequestHandler<ChangeUserEmailQuery>
	{
		private readonly EmailMessageFactory _emailMessageFactory;

		private readonly IPrincipal _principal;
		private readonly IEmailService _emailService;
		private readonly IUsersRepository _usersRepository;
		private readonly ILogger<ChangeUserEmailHandler> _logger;

		public ChangeUserEmailHandler(
			EmailMessageFactory emailMessageFactory,
			IPrincipal principal,
			IEmailService emailService,
			IUsersRepository usersRepository,
			ILogger<ChangeUserEmailHandler> logger
		)
		{
			_emailMessageFactory = emailMessageFactory ?? throw new ArgumentNullException(nameof(emailMessageFactory));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
			_usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Unit> Handle(ChangeUserEmailQuery request, CancellationToken cancellationToken)
		{
			if (String.IsNullOrWhiteSpace(request.Email))
			{
				throw new ValidationException($"{nameof(request.Email)} is invalid");
			}

			var userID = _principal.GetUserID();
			var user = await _usersRepository.GetAsync(userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var to = new EmailAddress(user.Email, user.UserName);
			var token = await _usersRepository.GenerateChangeEmailTokenAsync(user, request.Email);

			var emailMessage = _emailMessageFactory.CreateEmailChangingMessage(to, token);

			await _emailService.SendAsync(emailMessage);

			return Unit.Value;
		}
	}
}
