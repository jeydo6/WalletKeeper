using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Application.Commands
{
	public class DeleteReceiptHandler : IRequestHandler<DeleteReceiptCommand>
	{
		private readonly IReceiptsRepository _repository;
		private readonly ILogger<DeleteReceiptHandler> _logger;

		public DeleteReceiptHandler(
			IReceiptsRepository repository,
			ILogger<DeleteReceiptHandler> logger
		)
		{
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Unit> Handle(DeleteReceiptCommand request, CancellationToken cancellationToken)
		{
			if (request.ID <= 0)
			{
				throw new ValidationException($"{nameof(request.ID)} is invalid");
			}

			await _repository.DeleteAsync(request.ID, cancellationToken);

			return Unit.Value;
		}
	}
}
