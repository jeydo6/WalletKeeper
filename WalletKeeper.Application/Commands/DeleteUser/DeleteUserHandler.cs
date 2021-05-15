using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Extensions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Commands
{
	public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
	{
		private readonly IPrincipal _principal;
		private readonly IUsersRepository _usersRepository;
		private readonly ILogger<DeleteUserHandler> _logger;

		public DeleteUserHandler(
			IPrincipal principal,
			IUsersRepository usersRepository,
			ILogger<DeleteUserHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
		{
			var userID = _principal.GetUserID();

			await _usersRepository.DeleteAsync(userID);

			return Unit.Value;
		}
	}
}
