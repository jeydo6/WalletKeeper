using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using WalletKeeper.Application.Commands;
using WalletKeeper.Application.Dto;
using WalletKeeper.Application.Queries;

namespace WalletKeeper.WebAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/products")]
	public class ProductsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ProductsController(
			IMediator mediator
		)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		[HttpGet]
		[Produces(typeof(ProductDto[]))]
		public async Task<IActionResult> List()
		{
			return Ok(
				await _mediator.Send(new GetProductsQuery())
			);
		}

		[HttpPost]
		[Produces(typeof(ProductDto))]
		public async Task<IActionResult> Post(ProductDto dto)
		{
			return Ok(
				await _mediator.Send(new CreateProductCommand(dto))
			);
		}

		[HttpPut]
		[Produces(typeof(ProductDto))]
		public async Task<IActionResult> Put(ProductDto dto)
		{
			return Ok(
				await _mediator.Send(new UpdateProductCommand(dto))
			);
		}

		[HttpGet("{id}")]
		[Produces(typeof(ProductDto))]
		public async Task<IActionResult> Get(Int32 id)
		{
			return Ok(
				await _mediator.Send(new GetProductQuery(id))
			);
		}

		[HttpDelete("{id}")]
		[ProducesResponseType((Int32)HttpStatusCode.OK)]
		public async Task<IActionResult> Delete(Int32 id)
		{
			return Ok(
				await _mediator.Send(new DeleteProductCommand(id))
			);
		}
	}
}
