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
	[Route("products")]
	public class ProductsController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly ILogger<ProductsController> _logger;

		public ProductsController(
			ApplicationDbContext dbContext,
			ILogger<ProductsController> logger
		)
		{
			_dbContext = dbContext;
			_logger = logger;
		}

		[HttpGet]
		[Produces(typeof(ProductDto[]))]
		public async Task<IActionResult> List()
		{
			var products = await _dbContext.Products.ToListAsync();

			var result = products.Select(p => new ProductDto
			{
				ID = p.ID,
				Name = p.Name,
				CategoryID = p.CategoryID
			}).ToArray();

			return Ok(result);
		}

		[HttpPost]
		[Produces(typeof(ProductDto))]
		public async Task<IActionResult> Post(ProductDto dto)
		{
			if (dto == null)
			{
				throw new ValidationException($"{nameof(dto)} is invalid");
			}

			var product = await _dbContext.Products.FirstOrDefaultAsync(c => c.Name == dto.Name);
			if (product != null)
			{
				throw new BusinessException("Product already exists!");
			}

			product = new Product
			{
				Name = dto.Name,
				CategoryID = dto.CategoryID
			};

			await _dbContext.Products.AddAsync(product);
			await _dbContext.SaveChangesAsync();

			var result = new ProductDto
			{
				ID = product.ID,
				Name = product.Name,
				CategoryID = product.CategoryID
			};

			return Ok(result);
		}

		[HttpGet("{id}")]
		[Produces(typeof(ProductDto))]
		public async Task<IActionResult> Get(Int32 id)
		{
			if (id <= 0)
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			var product = await _dbContext.Products.FindAsync(id);
			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

			var result = new ProductDto
			{
				ID = product.ID,
				Name = product.Name,
				CategoryID = product.CategoryID
			};

			return Ok(result);
		}

		[HttpPut("{id}")]
		[Produces(typeof(ProductDto))]
		public async Task<IActionResult> Put(Int32 id, ProductDto dto)
		{
			if (id <= 0)
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			if (dto == null)
			{
				throw new ValidationException($"{nameof(dto)} is invalid");
			}

			var product = await _dbContext.Products.FindAsync(id);
			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

			product.Name = dto.Name;
			product.CategoryID = dto.CategoryID;
			product.Category = null;

			await _dbContext.SaveChangesAsync();

			var result = new ProductDto
			{
				ID = product.ID,
				Name = product.Name,
				CategoryID = product.CategoryID
			};

			return Ok(result);
		}

		[HttpDelete("{id}")]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> Delete(Int32 id)
		{
			if (id <= 0)
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			var product = await _dbContext.Products.FindAsync(id);
			if (product == null)
			{
				throw new BusinessException("Product is not exists!");
			}

			_dbContext.Products.Remove(product);
			await _dbContext.SaveChangesAsync();

			return NoContent();
		}
	}
}
