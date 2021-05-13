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
	[Route("authentication")]
	public class AuthenticationController : ControllerBase
	{
		private readonly IMediator _mediator;

		public AuthenticationController(
			IMediator mediator
		)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		[AllowAnonymous]
		[HttpPost]
		[Produces(typeof(String))]
		public async Task<IActionResult> GetToken(LoginDto dto)
		{
			return Ok(
				await _mediator.Send(new GetUserTokenQuery(dto))
			);
		}

		[HttpGet]
		[Produces(typeof(UserInfoDto))]
		public async Task<IActionResult> GetUserInfo()
		{
			return Ok(
				await _mediator.Send(new GetUserInfoQuery(User))
			);
		}
	}
}
