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
	[Route("api/users")]
	public class UsersController : ControllerBase
	{
		private readonly IMediator _mediator;

		public UsersController(
			IMediator mediator
		)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		[HttpPost]
		[AllowAnonymous]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> Post(CreateUserDto dto)
		{
			return Ok(
				await _mediator.Send(new CreateUserCommand(dto))
			);
		}

		[HttpGet]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> Get()
		{
			return Ok(
				await _mediator.Send(new GetUserQuery())
			);
		}

		[HttpDelete]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> Delete()
		{
			await _mediator.Send(new DeleteUserCommand());

			return NoContent();
		}

		[HttpPatch("change/userName")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ChangeUserName(ChangeUserNameDto dto)
		{
			return Ok(
				await _mediator.Send(new ChangeUserNameCommand(dto))
			);
		}

		[HttpPatch("change/password")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ChangePassword(ChangeUserPasswordDto dto)
		{
			return Ok(
				await _mediator.Send(new ChangeUserPasswordCommand(dto))
			);
		}

		[HttpGet("change/email")]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> ChangeEmail(String email)
		{
			await _mediator.Send(new ChangeUserEmailQuery(email));

			return NoContent();
		}

		[HttpPatch("change/email")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ChangeEmail(String token, ChangeUserEmailDto dto)
		{
			return Ok(
				await _mediator.Send(new ChangeUserEmailCommand(token, dto))
			);
		}

		[HttpGet("confirm/email")]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> ConfirmEmail()
		{
			await _mediator.Send(new ConfirmUserEmailQuery());

			return NoContent();
		}

		[HttpPatch("confirm/email")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ConfirmEmail(String token)
		{
			return Ok(
				await _mediator.Send(new ConfirmUserEmailCommand(token))
			);
		}

		[HttpGet("reset/password")]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> ResetPassword()
		{
			await _mediator.Send(new ResetUserPasswordQuery());

			return NoContent();
		}

		[HttpPatch("reset/password")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ResetPassword(String token, ResetUserPasswordDto dto)
		{
			return Ok(
				await _mediator.Send(new ResetUserPasswordCommand(token, dto))
			);
		}
	}
}
