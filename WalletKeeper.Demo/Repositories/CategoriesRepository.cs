using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Demo.DataContexts;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Demo.Repositories
{
	public class CategoriesRepository : ICategoriesRepository
	{
		private readonly ApplicationDataContext _dataContext;

		private readonly ILogger<CategoriesRepository> _logger;

		public CategoriesRepository(
			ApplicationDataContext dataContext,
			ILogger<CategoriesRepository> logger
		)
		{
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Category[]> GetAsync(CancellationToken cancellationToken = default)
		{
			var categories = _dataContext.Categories.ToArray();

			return await Task.FromResult(categories);
		}

		public async Task<Category> GetAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			var category = _dataContext.Categories.FirstOrDefault(c => c.ID == id);

			return await Task.FromResult(category);
		}

		public async Task<Category> CreateAsync(Category item, CancellationToken cancellationToken = default)
		{
			var category = _dataContext.Categories.FirstOrDefault(c => c.Name == item.Name);
			if (category != null)
			{
				throw new BusinessException("Category already exists!");
			}

			item.ID = _dataContext.Categories.Max(c => c.ID) + 1;
			_dataContext.Categories.Add(item);

			return await Task.FromResult(item);
		}

		public async Task<Category> UpdateAsync(Int32 id, Category item, CancellationToken cancellationToken = default)
		{
			var category = _dataContext.Categories.FirstOrDefault(c => c.ID == id);
			if (category == null)
			{
				throw new BusinessException("Category is not exists!");
			}

			category.Name = item.Name;

			return await Task.FromResult(category);
		}

		public async Task DeleteAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			var category = _dataContext.Categories.FirstOrDefault(c => c.ID == id);
			if (category == null)
			{
				throw new BusinessException("Category is not exists!");
			}

			foreach (var product in category.Products)
			{
				product.CategoryID = null;
				product.Category = null;
			}

			_dataContext.Categories.Remove(category);

			await Task.CompletedTask;
		}
	}
}
