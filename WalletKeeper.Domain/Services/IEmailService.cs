using System.Threading.Tasks;
using WalletKeeper.Domain.Types;

namespace WalletKeeper.Domain.Services
{
	public interface IEmailService
	{
		Task SendAsync(EmailMessage emailMessage);
	}
}
