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
	[Route("api/receipts")]
	public class ReceiptsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ReceiptsController(
			IMediator mediator
		)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		[AllowAnonymous]
		[HttpGet("fetch")]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> Fetch(GenericDto<String> dto)
		{
			return Ok(
				await _mediator.Send(new FetchReceiptQuery(dto.Value))
			);
		}

		[AllowAnonymous]
		[HttpPost("stringify")]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> Stringify(ReceiptDto dto)
		{
			return Ok(
				await _mediator.Send(new StringifyReceiptQuery(dto))
			);
		}

		[HttpGet]
		[Produces(typeof(ReceiptDto[]))]
		public async Task<IActionResult> List()
		{
			return Ok(
				await _mediator.Send(new GetReceiptsQuery())
			);
		}

		[HttpPost]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> Post(ReceiptDto dto)
		{
			return Ok(
				await _mediator.Send(new CreateReceiptCommand(dto))
			);
		}

		[HttpGet("{id}")]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> Get(Int32 id)
		{
			return Ok(
				await _mediator.Send(new GetReceiptQuery(id))
			);
		}

		[HttpDelete("{id}")]
		[ProducesResponseType((Int32)HttpStatusCode.OK)]
		public async Task<IActionResult> Delete(Int32 id)
		{
			return Ok(
				await _mediator.Send(new DeleteReceiptCommand(id))
			);
		}
	}
}
