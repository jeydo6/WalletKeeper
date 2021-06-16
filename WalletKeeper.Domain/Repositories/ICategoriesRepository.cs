using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Entities;

namespace WalletKeeper.Domain.Repositories
{
	public interface ICategoriesRepository
	{
		Task<Category[]> GetAsync(CancellationToken cancellationToken = default);

		Task<Category> GetAsync(Int32 id, CancellationToken cancellationToken = default);

		Task<Category> CreateAsync(Category item, CancellationToken cancellationToken = default);

		Task<Category> UpdateAsync(Category item, CancellationToken cancellationToken = default);

		Task DeleteAsync(Int32 id, CancellationToken cancellationToken = default);
	}
}
