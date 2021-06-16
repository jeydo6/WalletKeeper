using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Extensions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Commands
{
	public class DeleteProductHandler : IRequestHandler<DeleteProductCommand>
	{
		private readonly IPrincipal _principal;
		private readonly IProductsRepository _repository;
		private readonly ILogger<DeleteProductHandler> _logger;

		public DeleteProductHandler(
			IPrincipal principal,
			IProductsRepository repository,
			ILogger<DeleteProductHandler> logger
		)
		{
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			await _repository.DeleteAsync(request.ID, userID, cancellationToken);

			return Unit.Value;
		}
	}
}
