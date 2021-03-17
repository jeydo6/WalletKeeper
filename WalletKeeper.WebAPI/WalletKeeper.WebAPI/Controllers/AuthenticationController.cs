using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletKeeper.Application.Dto;
using WalletKeeper.Domain.Entities;
using WalletKeeper.WebAPI.Configs;

namespace WalletKeeper.WebAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("authentication")]
	public class AuthenticationController : ControllerBase
	{
		private readonly UserManager<User> _userManager;
		private readonly EndpointConfig _endpointConfig;
		private readonly IUserClaimsPrincipalFactory<User> _claimsPrincipalFactory;
		private readonly ILogger<AuthenticationController> _logger;

		public AuthenticationController(
			UserManager<User> userManager,
			IOptionsSnapshot<EndpointConfig> endpointConfigOptions,
			IUserClaimsPrincipalFactory<User> claimsPrincipalFactory,
			ILogger<AuthenticationController> logger
		)
		{
			_userManager = userManager;
			_endpointConfig = endpointConfigOptions.Value;
			_claimsPrincipalFactory = claimsPrincipalFactory;
			_logger = logger;
		}

		[AllowAnonymous]
		[HttpPost]
		[Produces(typeof(String))]
		public async Task<IActionResult> GetTokenAsync(LoginDto login)
		{
			if (String.IsNullOrEmpty(login.UserName))
			{
				return BadRequest("UserName cannot be empty!");
			}

			if (String.IsNullOrEmpty(login.Password))
			{
				return BadRequest("Password cannot be empty!");
			}

			var user = await _userManager.FindByNameAsync(login.UserName);
			if (user == null || !await _userManager.CheckPasswordAsync(user, login.Password))
			{
				return BadRequest("Invalid username or password");
			}

			var identity = await _claimsPrincipalFactory.CreateAsync(user);
			if (identity == null)
			{
				return BadRequest("Invalid username or password");
			}

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_endpointConfig.Secret)
			);

			var jwt = new JwtSecurityToken(
				issuer: _endpointConfig.Issuer,
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddDays(14),
				claims: identity.Claims,
				signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			);

			return Ok(
				new JwtSecurityTokenHandler()
					.WriteToken(jwt)
			);
		}

		[HttpGet]
		[Produces(typeof(UserInfoDto))]
		public IActionResult GetClaims()
		{
			var userInfo = new UserInfoDto
			{
				IsAuthenticated = true,
				Claims = User.Claims
					.Select(c => new ClaimDto
					{
						Type = c.Type,
						Value = c.Value,
						ValueType = c.ValueType,
						Issuer = c.Issuer,
						OriginalIssuer = c.OriginalIssuer
					})
					.ToArray()
			};

			return Ok(userInfo);
		}
	}
}
