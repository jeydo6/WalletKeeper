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
	[Route("api/categories")]
	public class CategoriesController : ControllerBase
	{
		private readonly IMediator _mediator;

		public CategoriesController(
			IMediator mediator
		)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		[HttpGet]
		[Produces(typeof(CategoryDto[]))]
		public async Task<IActionResult> List()
		{
			return Ok(
				await _mediator.Send(new GetCategoriesQuery())
			);
		}

		[HttpPost]
		[Produces(typeof(CategoryDto))]
		public async Task<IActionResult> Post(CategoryDto dto)
		{
			return Ok(
				await _mediator.Send(new CreateCategoryCommand(dto))
			);
		}

		[HttpPut]
		[Produces(typeof(CategoryDto))]
		public async Task<IActionResult> Put(CategoryDto dto)
		{
			return Ok(
				await _mediator.Send(new UpdateCategoryCommand(dto))
			);
		}

		[HttpGet("{id}")]
		[Produces(typeof(CategoryDto))]
		public async Task<IActionResult> Get(Int32 id)
		{
			return Ok(
				await _mediator.Send(new GetCategoryQuery(id))
			);
		}

		[HttpDelete("{id}")]
		[ProducesResponseType((Int32)HttpStatusCode.OK)]
		public async Task<IActionResult> Delete(Int32 id)
		{
			return Ok(
				await _mediator.Send(new DeleteCategoryCommand(id))
			);
		}
	}
}
