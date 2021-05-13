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
	[Route("receipts")]
	public class ReceiptsController : ControllerBase
	{
		private readonly IMediator _mediator;

		public ReceiptsController(
			IMediator mediator
		)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}


		[HttpPost("photo")]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> Post(GenericDto<Byte[]> dto)
		{
			var barcodeString = await _mediator.Send(new DecodeQRCodeQuery(dto.Value));

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
	}
}
