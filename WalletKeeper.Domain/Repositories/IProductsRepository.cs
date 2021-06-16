using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Entities;

namespace WalletKeeper.Domain.Repositories
{
	public interface IProductsRepository
	{
		Task<Product[]> GetAsync(CancellationToken cancellationToken = default);

		Task<Product> GetAsync(Int32 id, CancellationToken cancellationToken = default);

		Task<Product> CreateAsync(Product item, CancellationToken cancellationToken = default);

		Task<Product> UpdateAsync(Product item, CancellationToken cancellationToken = default);

		Task DeleteAsync(Int32 id, CancellationToken cancellationToken = default);
	}
}
