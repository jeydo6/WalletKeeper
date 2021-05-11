using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Factories;
using WalletKeeper.Domain.Services;
using WalletKeeper.Domain.Types;
using WalletKeeper.WebAPI.Extensions;

namespace WalletKeeper.WebAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("users")]
	public class UsersController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly EmailMessageFactory _emailMessageFactory;
		
		private readonly IEmailService _emailService;
		private readonly ILogger<UsersController> _logger;

		public UsersController(
			UserManager<User> userManager,
			EmailMessageFactory emailMessageFactory,
			IEmailService emailService,
			ILogger<UsersController> logger
		)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_emailMessageFactory = emailMessageFactory ?? throw new ArgumentNullException(nameof(emailMessageFactory));
			_emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
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

		[HttpGet]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> Get()
		{
			var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (String.IsNullOrWhiteSpace(userID))
			{
				throw new ValidationException($"{nameof(userID)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(userID);
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

		[HttpDelete]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> Delete()
		{
			var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (String.IsNullOrWhiteSpace(userID))
			{
				throw new ValidationException($"{nameof(userID)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var identityResult = await _userManager.DeleteAsync(user);
			identityResult.EnsureSuccess("An error occurred while deleting a user", _logger);

			return NoContent();
		}

		[HttpPatch("change/userName")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ChangeUserName(ChangeUserNameDto dto)
		{
			var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (String.IsNullOrWhiteSpace(userID))
			{
				throw new ValidationException($"{nameof(userID)} is invalid");
			}

			if (dto == null)
			{
				throw new ValidationException($"{nameof(dto)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(dto.UserName))
			{
				throw new ValidationException($"{nameof(dto.UserName)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(userID);
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

		[HttpPatch("change/password")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ChangePassword(ChangeUserPasswordDto dto)
		{
			var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (String.IsNullOrWhiteSpace(userID))
			{
				throw new ValidationException($"{nameof(userID)} is invalid");
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

			var user = await _userManager.FindByIdAsync(userID);
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

		[HttpGet("change/email")]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> ChangeEmail(String newEmail)
		{
			var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (String.IsNullOrWhiteSpace(userID))
			{
				throw new ValidationException($"{nameof(userID)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(newEmail))
			{
				throw new ValidationException($"{nameof(newEmail)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var to = new EmailAddress(user.Email, user.UserName);
			var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

			var emailMessage = _emailMessageFactory.CreateEmailChangingMessage(to, token);

			await _emailService.SendAsync(emailMessage);

			return NoContent();
		}

		[HttpPatch("change/email")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ChangeEmail(String token, ChangeUserEmailDto dto)
		{
			var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (String.IsNullOrWhiteSpace(userID))
			{
				throw new ValidationException($"{nameof(userID)} is invalid");
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

			var user = await _userManager.FindByIdAsync(userID);
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

		[HttpGet("confirm/email")]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> ConfirmEmail()
		{
			var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (String.IsNullOrWhiteSpace(userID))
			{
				throw new ValidationException($"{nameof(userID)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var to = new EmailAddress(user.Email, user.UserName);
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			var emailMessage = _emailMessageFactory.CreateEmailConfirmationMessage(to, token);

			await _emailService.SendAsync(emailMessage);

			return NoContent();
		}

		[HttpPatch("confirm/email")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ConfirmEmail(String token)
		{
			var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (String.IsNullOrWhiteSpace(userID))
			{
				throw new ValidationException($"{nameof(userID)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(userID);
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

		[HttpGet("reset/password")]
		[ProducesResponseType((Int32)HttpStatusCode.NoContent)]
		public async Task<IActionResult> ResetPassword()
		{
			var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (String.IsNullOrWhiteSpace(userID))
			{
				throw new ValidationException($"{nameof(userID)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var to = new EmailAddress(user.Email, user.UserName);
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);

			var emailMessage = _emailMessageFactory.CreatePasswordResettingMessage(to, token);

			await _emailService.SendAsync(emailMessage);

			return NoContent();
		}

		[HttpPatch("reset/password")]
		[Produces(typeof(UserDto))]
		public async Task<IActionResult> ResetPassword(String token, ResetUserPasswordDto dto)
		{
			var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (String.IsNullOrWhiteSpace(userID))
			{
				throw new ValidationException($"{nameof(userID)} is invalid");
			}

			if (dto == null)
			{
				throw new ValidationException($"{nameof(dto)} is invalid");
			}

			if (String.IsNullOrWhiteSpace(dto.Password))
			{
				throw new ValidationException($"{nameof(dto.Password)} is invalid");
			}

			var user = await _userManager.FindByIdAsync(userID);
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
