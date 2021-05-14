using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.Persistence.Repositories
{
	public class CategoriesRepository : ICategoriesRepository
	{
		private readonly ApplicationDbContext _dbContext;

		private readonly ILogger<CategoriesRepository> _logger;

		public CategoriesRepository(
			ApplicationDbContext dbContext,
			ILogger<CategoriesRepository> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<Category[]> GetAsync(CancellationToken cancellationToken = default)
		{
			var categories = await _dbContext.Categories
				.ToArrayAsync(cancellationToken);

			return categories;
		}

		public async Task<Category> GetAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			var category = await _dbContext.Categories.FindAsync(new Object[] { id }, cancellationToken);
			if (category == null)
			{
				throw new BusinessException("Category is not exists!");
			}

			return category;
		}

		public async Task<Category> CreateAsync(Category item, CancellationToken cancellationToken = default)
		{
			var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == item.Name, cancellationToken);
			if (category != null)
			{
				throw new BusinessException("Category already exists!");
			}

			category = new Category
			{
				Name = item.Name
			};

			await _dbContext.Categories.AddAsync(item, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);

			return category;
		}

		public async Task<Category> UpdateAsync(Int32 id, Category item, CancellationToken cancellationToken = default)
		{
			var category = await _dbContext.Categories.FindAsync(new Object[] { id }, cancellationToken);
			if (category == null)
			{
				throw new BusinessException("Category is not exists!");
			}

			category.Name = item.Name;

			await _dbContext.SaveChangesAsync(cancellationToken);

			return category;
		}

		public async Task DeleteAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			var category = await _dbContext.Categories.FindAsync(new Object[] { id }, cancellationToken);
			if (category == null)
			{
				throw new BusinessException("Category is not exists!");
			}

			_dbContext.Categories.Remove(category);
			await _dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
