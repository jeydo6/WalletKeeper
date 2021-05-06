using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.WebAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("categories")]
	public class CategoriesController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly ILogger<CategoriesController> _logger;

		public CategoriesController(
			ApplicationDbContext dbContext,
			ILogger<CategoriesController> logger
		)
		{
			_dbContext = dbContext;
			_logger = logger;
		}

		[HttpGet]
		[Produces(typeof(CategoryDto[]))]
		public async Task<IActionResult> List()
		{
			var categories = await _dbContext.Categories.ToListAsync();

			var result = categories.Select(c => new CategoryDto
			{
				ID = c.ID,
				Name = c.Name
			}).ToArray();

			return Ok(result);
		}

		[HttpGet("{id}")]
		[Produces(typeof(CategoryDto))]
		public async Task<IActionResult> Get(Int32 id)
		{
			try
			{
				if (id <= 0)
				{
					throw new BusinessException($"'{nameof(id)}' is invalid");
				}

				var category = await _dbContext.Categories.FindAsync(id);
				if (category == null)
				{
					throw new BusinessException("Category is not exists!");
				}

				var result = new CategoryDto
				{
					ID = category.ID,
					Name = category.Name
				};

				return Ok(result);
			}
			catch (BusinessException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Produces(typeof(CategoryDto))]
		public async Task<IActionResult> Post(CategoryDto dto)
		{
			try
			{
				if (dto == null)
				{
					throw new BusinessException($"'{nameof(dto)}' is invalid");
				}

				var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == dto.Name);
				if (category != null)
				{
					throw new BusinessException("Category already exists!");
				}

				category = new Category
				{
					Name = dto.Name
				};

				await _dbContext.Categories.AddAsync(category);
				await _dbContext.SaveChangesAsync();

				var result = new CategoryDto
				{
					ID = category.ID,
					Name = category.Name
				};

				return Ok(result);
			}
			catch (BusinessException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id}")]
		[Produces(typeof(CategoryDto))]
		public async Task<IActionResult> Put(Int32 id, CategoryDto dto)
		{
			try
			{
				if (id <= 0)
				{
					throw new BusinessException($"'{nameof(id)}' is invalid");
				}

				if (dto == null)
				{
					throw new BusinessException($"'{nameof(dto)}' is invalid");
				}

				var category = await _dbContext.Categories.FindAsync(id);
				if (category == null)
				{
					throw new BusinessException("Category is not exists!");
				}

				category.Name = dto.Name;

				await _dbContext.SaveChangesAsync();

				var result = new CategoryDto
				{
					ID = category.ID,
					Name = category.Name
				};

				return Ok(result);
			}
			catch (BusinessException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> Delete(Int32 id)
		{
			try
			{
				if (id <= 0)
				{
					throw new BusinessException($"'{nameof(id)}' is invalid");
				}

				var category = await _dbContext.Categories.FindAsync(id);
				if (category == null)
				{
					throw new BusinessException("Category is not exists!");
				}

				_dbContext.Categories.Remove(category);
				await _dbContext.SaveChangesAsync();

				return NoContent();
			}
			catch (BusinessException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
