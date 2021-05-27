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

		[HttpGet]
		[Produces(typeof(ReceiptDto[]))]
		public async Task<IActionResult> List()
		{
			return Ok(
				await _mediator.Send(new GetReceiptsQuery())
			);
		}

		[HttpPost("photo")]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> Post(GenericDto<Byte[]> dto)
		{
			var barcodeString = await _mediator.Send(new GetQRCodeStringQuery(dto.Value));

			return Ok(
				await _mediator.Send(new CreateReceiptCommand(barcodeString))
			);
		}

		[HttpPost("barcode")]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> Post(GenericDto<String> dto)
		{
			var barcodeString = dto.Value;

			return Ok(
				await _mediator.Send(new CreateReceiptCommand(barcodeString))
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
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> Delete(Int32 id)
		{
			await _mediator.Send(new DeleteReceiptCommand(id));

			return NoContent();
		}
	}
}
