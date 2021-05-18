using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using WalletKeeper.Demo.DataContexts;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Extensions;
using WalletKeeper.Domain.Repositories;

namespace WalletKeeper.Demo.Repositories
{
	public class UsersRepository : IUsersRepository
	{
		private readonly ApplicationDataContext _dataContext;

		private readonly IPrincipal _principal;
		private readonly ILogger<UsersRepository> _logger;

		public UsersRepository(
			ApplicationDataContext dataContext,
			IPrincipal principal,
			ILogger<UsersRepository> logger
		)
		{
			_dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
			_principal = principal ?? throw new ArgumentNullException(nameof(principal));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<User> GetAsync(String id)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var user = _dataContext.Users.FirstOrDefault(u => u.Id == userID);

			return await Task.FromResult(user);
		}

		public async Task<User> FindAsync(String userName)
		{
			var user = _dataContext.Users.FirstOrDefault(u => u.UserName == userName);

			return await Task.FromResult(user);
		}

		public async Task<User> CreateAsync(User item, String password)
		{
			var user = _dataContext.Users.FirstOrDefault(u => u.NormalizedUserName == item.UserName.ToUpper() || u.NormalizedEmail == u.Email.ToUpper());
			if (user != null)
			{
				throw new BusinessException("User already exists!");
			}

			item.Id = Guid.NewGuid();
			item.PasswordHash = password;
			_dataContext.Users.Add(item);

			var role = _dataContext.Roles.FirstOrDefault(r => r.Name == "User");
			if (role != null)
			{
				var userRole = (item.Id, role.Id);
				_dataContext.UserRoles.Add(userRole);
			}

			return await Task.FromResult(item);
		}

		public async Task DeleteAsync(String id)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var user = _dataContext.Users.FirstOrDefault(u => u.Id == userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			_dataContext.Users.Remove(user);

			var userRoles = _dataContext.UserRoles
				.Where(ur => ur.UserId == userID)
				.ToList();

			foreach (var userRole in userRoles)
			{
				_dataContext.UserRoles.Remove(userRole);
			}

			await Task.CompletedTask;
		}

		public async Task<Boolean> CheckPasswordAsync(User user, String password)
		{
			var result = user.PasswordHash == password;

			return await Task.FromResult(result);
		}

		public async Task<User> ChangeUserNameAsync(String id, String userName)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var user = _dataContext.Users.FirstOrDefault(u => u.Id == userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			user.UserName = userName;
			user.NormalizedUserName = userName.ToUpper();

			return await Task.FromResult(user);
		}

		public async Task<User> ChangePasswordAsync(String id, String oldPassword, String newPassword)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var user = _dataContext.Users.FirstOrDefault(u => u.Id == userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			if (user.PasswordHash == oldPassword)
			{
				user.PasswordHash = newPassword;
			}
			else
			{
				throw new BusinessException($"{nameof(oldPassword)} is invalid");
			}

			return await Task.FromResult(user);
		}

		public async Task<User> ChangeEmailAsync(String id, String email, String token)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var user = _dataContext.Users.FirstOrDefault(u => u.Id == userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			user.Email = email;
			user.NormalizedEmail = email.ToUpper();
			user.EmailConfirmed = true;

			return await Task.FromResult(user);
		}

		public async Task<User> ConfirmEmailAsync(String id, String token)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var user = _dataContext.Users.FirstOrDefault(u => u.Id == userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			user.EmailConfirmed = true;

			return await Task.FromResult(user);
		}

		public async Task<User> ResetPasswordAsync(String id, String password, String token)
		{
			if (!Guid.TryParse(_principal.GetUserID(), out var userID))
			{
				throw new BusinessException($"{nameof(userID)} is invalid");
			}

			var user = _dataContext.Users.FirstOrDefault(u => u.Id == userID);
			if (user == null)
			{
				throw new BusinessException("User is not exists!");
			}

			user.PasswordHash = password;

			return await Task.FromResult(user);
		}

		public async Task<String> GenerateChangeEmailTokenAsync(User user, String email)
		{
			return await Task.FromResult<String>(null);
		}

		public async Task<String> GenerateConfirmEmailTokenAsync(User user)
		{
			return await Task.FromResult<String>(null);
		}

		public async Task<String> GeneratePasswordResetTokenAsync(User user)
		{
			return await Task.FromResult<String>(null);
		}
	}
}
