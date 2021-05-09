using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.WebAPI.Extensions;

namespace WalletKeeper.WebAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("users")]
	public class UsersController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly ILogger<UsersController> _logger;

		public UsersController(
			UserManager<User> userManager,
			ILogger<UsersController> logger
		)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[HttpPost]
		[AllowAnonymous]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> Post(CreateUserDto dto)
		{
			if (dto == null)
			{
				throw new ValidationException($"{nameof(dto)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(dto.UserName))
			{
				throw new ValidationException($"{nameof(dto.UserName)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(dto.Password))
			{
				throw new ValidationException($"{nameof(dto.Password)} is invalid");
			}

			var user = new User
			{
				UserName = dto.UserName,
				Email = dto.Email,
				EmailConfirmed = false
			};

			var identityResult = await _userManager.CreateAsync(user, dto.Password);
			identityResult.EnsureSuccess("An error occurred while creating a user", _logger);

			var result = new UserDto
			{
				ID = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				EmailConfirmed = user.EmailConfirmed
			};

			return Ok(result);
		}

		[HttpGet("{id}")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> Get(String id)
		{
			if (String.IsNullOrWhiteSpace(id))
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var result = new UserDto
			{
				ID = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				EmailConfirmed = user.EmailConfirmed
			};

			return Ok(result);
		}

		[HttpDelete("{id}")]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> Delete(String id)
		{
			if (String.IsNullOrWhiteSpace(id))
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var identityResult = await _userManager.DeleteAsync(user);
			identityResult.EnsureSuccess("An error occurred while deleting a user", _logger);

			return NoContent();
		}

		[HttpPatch("{id}/change/userName")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ChangeUserName(String id, ChangeUserNameDto dto)
		{
			if (String.IsNullOrWhiteSpace(id))
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			if (dto == null)
			{
				throw new ValidationException($"{nameof(dto)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(dto.UserName))
			{
				throw new ValidationException($"{nameof(dto.UserName)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var identityResult = await _userManager.SetUserNameAsync(user, dto.UserName);
			identityResult.EnsureSuccess("An error occurred while patching a user", _logger);

			var result = new UserDto
			{
				ID = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				EmailConfirmed = user.EmailConfirmed
			};

			return Ok(result);
		}

		[HttpPatch("{id}/change/password")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ChangePassword(String id, ChangeUserPasswordDto dto)
		{
			if (String.IsNullOrWhiteSpace(id))
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			if (dto == null)
			{
				throw new ValidationException($"{nameof(dto)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(dto.OldPassword))
			{
				throw new ValidationException($"{nameof(dto.OldPassword)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(dto.NewPassword))
			{
				throw new ValidationException($"{nameof(dto.NewPassword)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var identityResult = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
			identityResult.EnsureSuccess("An error occurred while patching a user", _logger);

			var result = new UserDto
			{
				ID = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				EmailConfirmed = user.EmailConfirmed
			};

			return Ok(result);
		}

		[HttpGet("{id}/change/email")]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> ChangeEmail(String id, ChangeUserEmailDto dto)
		{
			if (String.IsNullOrWhiteSpace(id))
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			if (dto == null)
			{
				throw new ValidationException($"{nameof(dto)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(dto.Email))
			{
				throw new ValidationException($"{nameof(dto.Email)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.Email);

			return NoContent();
		}

		[HttpPatch("{id}/change/email")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ChangeEmail(String id, String token, ChangeUserEmailDto dto)
		{
			if (String.IsNullOrWhiteSpace(id))
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			if (dto == null)
			{
				throw new ValidationException($"{nameof(dto)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(dto.Email))
			{
				throw new ValidationException($"{nameof(dto.Email)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(token))
			{
				throw new ValidationException($"{nameof(token)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var identityResult = await _userManager.ChangeEmailAsync(user, dto.Email, token);
			identityResult.EnsureSuccess("An error occurred while patching a user", _logger);

			var result = new UserDto
			{
				ID = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				EmailConfirmed = user.EmailConfirmed
			};

			return Ok(result);
		}

		[HttpGet("{id}/confirm/email")]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> ConfirmEmail(String id)
		{
			if (String.IsNullOrWhiteSpace(id))
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			return NoContent();
		}

		[HttpPatch("{id}/confirm/email")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ConfirmEmail(String id, String token)
		{
			if (String.IsNullOrWhiteSpace(id))
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var identityResult = await _userManager.ConfirmEmailAsync(user, token);
			identityResult.EnsureSuccess("An error occurred while patching a user", _logger);

			var result = new UserDto
			{
				ID = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				EmailConfirmed = user.EmailConfirmed
			};

			return Ok(result);
		}

		[HttpGet("{id}/reset/password")]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> ResetPassword(String id)
		{
			if (String.IsNullOrWhiteSpace(id))
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);

			return NoContent();
		}

		[HttpPatch("{id}/reset/password")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ResetPassword(String id, String token, ResetUserPasswordDto dto)
		{
			if (String.IsNullOrWhiteSpace(id))
			{
				throw new ValidationException($"{nameof(id)} is invalid");
			}

			if (dto == null)
			{
				throw new ValidationException($"{nameof(dto)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(dto.Password))
			{
				throw new ValidationException($"{nameof(dto.Password)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var identityResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);
			identityResult.EnsureSuccess("An error occurred while patching a user", _logger);

			var result = new UserDto
			{
				ID = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				EmailConfirmed = user.EmailConfirmed
			};

			return Ok(result);
		}

	}
}
