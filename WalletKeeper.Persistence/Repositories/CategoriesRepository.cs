using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;
using WalletKeeper.Domain.Types;
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
			var result = await _dbContext.Categories
				.Select(c => new Category
				{
					ID = c.ID,
					Name = c.Name
				})
				.ToArrayAsync(cancellationToken);

			return result;
		}

		public async Task<Category> GetAsync(Int32 id, CancellationToken cancellationToken = default)
		{
			var category = await _dbContext.Categories.FindAsync(new Object[] { id }, cancellationToken);
			if (category == null)
			{
				throw new BusinessException("Category is not exists!");
			}

			var result = new Category
			{
				ID = category.ID,
				Name = category.Name
			};

			return result;
		}

		public async Task<Category> CreateAsync(Category item, CancellationToken cancellationToken = default)
		{
			var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == item.Name, cancellationToken);
			if (category != null)
			{
				throw new BusinessException("Category already exists!");
			}

			category = new Entities.Category
			{
				Name = item.Name
			};

			await _dbContext.Categories.AddAsync(category, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);

			var result = new Category
			{
				ID = category.ID,
				Name = category.Name
			};

			return result;
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

			var result = new Category
			{
				ID = category.ID,
				Name = category.Name
			};

			return result;
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
