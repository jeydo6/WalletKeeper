using System;
using System.Threading.Tasks;
using WalletKeeper.Domain.Services;
using WalletKeeper.Domain.Types;

namespace WalletKeeper.Infrastructure.Services
{
	public class EmailService : IEmailService
	{
		public Task Send(EmailMessage message)
		{
			throw new NotImplementedException();
		}
	}
}
