using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Repositories;
using WalletKeeper.Persistence.Extensions;

namespace WalletKeeper.Persistence.Repositories
{
	public class UsersRepository : IUsersRepository
	{
		private readonly UserManager<User> _userManager;

		private readonly ILogger<UsersRepository> _logger;

		public UsersRepository(
			UserManager<User> userManager,
			ILogger<UsersRepository> logger
		)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<User> GetAsync(String id)
		{
			var user = await _userManager.FindByIdAsync(id);

			return user;
		}

		public async Task<User> FindAsync(String userName)
		{
			var user = await _userManager.FindByNameAsync(userName);

			return user;
		}

		public async Task<User> CreateAsync(User item, String password)
		{
			var result = await _userManager.CreateAsync(item, password);
			result.EnsureSuccess("An error occurred while creating a user", _logger);

			result = await _userManager.AddToRoleAsync(item, "User");
			result.EnsureSuccess("An error occurred while creating a user", _logger);

			return item;
		}

		public async Task DeleteAsync(String id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var result = await _userManager.DeleteAsync(user);
			result.EnsureSuccess("An error occurred while deleting a user", _logger);
		}

		public async Task<Boolean> CheckPasswordAsync(User user, String password)
		{
			var result = await _userManager.CheckPasswordAsync(user, password);

			return result;
		}

		public async Task<User> ChangeUserNameAsync(String id, String userName)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var result = await _userManager.SetUserNameAsync(user, userName);
			result.EnsureSuccess("An error occurred while patching a user", _logger);

			return user;
		}

		public async Task<User> ChangePasswordAsync(String id, String oldPassword, String newPassword)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
			result.EnsureSuccess("An error occurred while patching a user", _logger);

			return user;
		}

		public async Task<User> ChangeEmailAsync(String id, String email, String token)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var result = await _userManager.ChangeEmailAsync(user, email, token);
			result.EnsureSuccess("An error occurred while patching a user", _logger);

			return user;
		}

		public async Task<User> ConfirmEmailAsync(String id, String token)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var result = await _userManager.ConfirmEmailAsync(user, token);
			result.EnsureSuccess("An error occurred while patching a user", _logger);

			return user;
		}

		public async Task<User> ResetPasswordAsync(String id, String password, String token)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			var result = await _userManager.ResetPasswordAsync(user, token, password);
			result.EnsureSuccess("An error occurred while patching a user", _logger);

			return user;
		}

		public async Task<String> GenerateChangeEmailTokenAsync(User user, String email)
		{
			var token = await _userManager.GenerateChangeEmailTokenAsync(user, email);

			return token;
		}

		public async Task<String> GenerateConfirmEmailTokenAsync(User user)
		{
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			return token;
		}

		public async Task<String> GeneratePasswordResetTokenAsync(User user)
		{
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);

			return token;
		}
	}
}
