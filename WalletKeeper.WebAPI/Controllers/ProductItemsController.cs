using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WalletKeeper.Application.Commands;
using WalletKeeper.Application.Dto;
using WalletKeeper.Application.Queries;

namespace WalletKeeper.WebAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/productItems")]
	public class ProductItemsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ProductItemsController(
			IMediator mediator
		)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		[HttpGet]
		[Produces(typeof(ProductItemDto[]))]
		public async Task<IActionResult> List()
		{
			return Ok(
				await _mediator.Send(new GetProductItemsQuery())
			);
		}

		[HttpPut]
		[Produces(typeof(ProductItemDto))]
		public async Task<IActionResult> Put(ProductItemDto dto)
		{
			return Ok(
				await _mediator.Send(new UpdateProductItemCommand(dto))
			);
		}

		[HttpGet("{id}")]
		[Produces(typeof(ProductItemDto))]
		public async Task<IActionResult> Get(Int32 id)
		{
			return Ok(
				await _mediator.Send(new GetProductItemQuery(id))
			);
		}
	}
}
