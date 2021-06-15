using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Entities;

namespace WalletKeeper.Domain.Repositories
{
	public interface IReceiptsRepository
	{
		Task<Receipt[]> GetAsync(CancellationToken cancellationToken = default);

		Task<Receipt> GetAsync(Int32 id, CancellationToken cancellationToken = default);

		Task<Receipt> FindAsync(String fiscalDocumentNumber, String fiscalDriveNumber, CancellationToken cancellationToken = default);

		Task<Receipt> CreateAsync(Receipt item, CancellationToken cancellationToken = default);

		Task DeleteAsync(Int32 id, CancellationToken cancellationToken = default);
	}
}
