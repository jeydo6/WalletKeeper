using System;
using System.Threading.Tasks;
using WalletKeeper.Domain.Entities;

namespace WalletKeeper.Domain.Repositories
{
	public interface IUsersRepository
	{
		Task<User> GetAsync(String id);

		Task<User> FindAsync(String userName);

		Task<User> CreateAsync(User item, String password);

		Task DeleteAsync(String id);

		Task<Boolean> CheckPasswordAsync(User user, String password);

		Task<User> ChangeUserNameAsync(String id, String userName);

		Task<User> ChangePasswordAsync(String id, String oldPassword, String newPassword);

		Task<User> ChangeEmailAsync(String id, String email, String token);

		Task<User> ConfirmEmailAsync(String id, String token);

		Task<User> ResetPasswordAsync(String id, String password, String token);

		Task<String> GenerateChangeEmailTokenAsync(User user, String email);

		Task<String> GenerateConfirmEmailTokenAsync(User user);

		Task<String> GeneratePasswordResetTokenAsync(User user);
	}
}
