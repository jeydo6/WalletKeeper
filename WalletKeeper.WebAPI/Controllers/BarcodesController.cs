using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Application.Queries;

namespace WalletKeeper.WebAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/barcodes")]
	public class BarcodesController : ControllerBase
	{
		private readonly IMediator _mediator;

		public BarcodesController(
			IMediator mediator
		)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		[AllowAnonymous]
		[HttpPost("decode")]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> Decode(GenericDto<Byte[]> dto)
		{
			return Ok(
				await _mediator.Send(new DecodeReceiptQuery(dto.Value))
			);
		}

		[AllowAnonymous]
		[HttpPost("encode")]
		[Produces(typeof(ReceiptDto))]
		public async Task<IActionResult> Encode(GenericDto<String> dto)
		{
			return Ok(
				await _mediator.Send(new EncodeReceiptQuery(dto.Value))
			);
		}
	}
}
