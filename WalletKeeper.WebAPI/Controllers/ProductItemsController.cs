using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Persistence.DbContexts;

namespace WalletKeeper.WebAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("productItems")]
	public class ProductItemsController : ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly ILogger<ProductItemsController> _logger;

		public ProductItemsController(
			ApplicationDbContext dbContext,
			ILogger<ProductItemsController> logger
		)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpGet]
		[Produces(typeof(ProductItemDto[]))]
		public async Task<IActionResult> List()
		{
			if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userID))
			{
				throw new BusinessException("userID is invalid");
			}

			var result = await _dbContext.ProductItems
				.Where(pi => pi.Receipt.UserID == userID)
				.Select(pi => new ProductItemDto
				{
					ID = pi.ID,
					Name = pi.Name,
					Price = pi.Price,
					Quantity = pi.Quantity,
					Sum = pi.Sum,
					ProductID = pi.ProductID,
					ReceiptID = pi.ReceiptID
				})
				.ToArrayAsync();

			return Ok(result);
		}

		[HttpGet("{id}")]
		[Produces(typeof(ProductItemDto))]
		public async Task<IActionResult> Get(Int32 id)
		{
			if (id <= 0)
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userID))
			{
				throw new BusinessException("userID is invalid");
			}

			var productItem = await _dbContext.ProductItems.FirstOrDefaultAsync(pi => pi.ID == id && pi.Receipt.UserID == userID);
			if (productItem == null)
			{
				throw new BusinessException("ProductItem is not exists!");
			}

			var result = new ProductItemDto
			{
				ID = productItem.ID,
				Name = productItem.Name,
				Price = productItem.Price,
				Quantity = productItem.Quantity,
				Sum = productItem.Sum,
				ProductID = productItem.ProductID,
				ReceiptID = productItem.ReceiptID
			};

			return Ok(result);
		}

		[HttpPut("{id}")]
		[Produces(typeof(ProductItemDto))]
		public async Task<IActionResult> Put(Int32 id, ProductItemDto dto)
		{
			if (id <= 0)
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			if (dto == null)
			{
				throw new ValidationException($"{nameof(dto)} is invalid");
			}

			if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userID))
			{
				throw new BusinessException("userID is invalid");
			}

			var productItem = await _dbContext.ProductItems.FirstOrDefaultAsync(pi => pi.ID == id && pi.Receipt.UserID == userID);
			if (productItem == null)
			{
				throw new BusinessException("ProductItem is not exists!");
			}

			productItem.Name = dto.Name;
			productItem.ProductID = dto.ProductID;
			productItem.Product = null;

			await _dbContext.SaveChangesAsync();

			var result = new ProductItemDto
			{
				ID = productItem.ID,
				Name = productItem.Name,
				Price = productItem.Price,
				Quantity = productItem.Quantity,
				Sum = productItem.Sum,
				ProductID = productItem.ProductID,
				ReceiptID = productItem.ReceiptID
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

			if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userID))
			{
				throw new BusinessException("userID is invalid");
			}

			var productItem = await _dbContext.ProductItems.FirstOrDefaultAsync(pi => pi.ID == id && pi.Receipt.UserID == userID);
			if (productItem == null)
			{
				throw new BusinessException("ProductItem is not exists!");
			}

			_dbContext.ProductItems.Remove(productItem);
			await _dbContext.SaveChangesAsync();

			return NoContent();
		}
	}
}
