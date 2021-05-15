using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Entities;

namespace WalletKeeper.Domain.Repositories
{
	public interface IProductItemsRepository
	{
		Task<ProductItem[]> GetAsync(Guid userID, CancellationToken cancellationToken = default);

		Task<ProductItem> GetAsync(Int32 id, Guid userID, CancellationToken cancellationToken = default);

		Task<ProductItem> UpdateAsync(Int32 id, ProductItem item, Guid userID, CancellationToken cancellationToken = default);
	}
}
