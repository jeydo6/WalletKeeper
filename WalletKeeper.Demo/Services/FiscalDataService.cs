using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletKeeper.Demo.DataContexts;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Providers;
using WalletKeeper.Domain.Services;
using WalletKeeper.Domain.Types;

namespace WalletKeeper.Demo.Services
{
	public class FiscalDataService : IFiscalDataService
	{
		private static readonly Random Random = new(0);

		private readonly ApplicationDataContext _dataContext;

		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly ILogger<FiscalDataService> _logger;

		public FiscalDataService(
			ApplicationDataContext dataContext,
			IDateTimeProvider dateTimeProvider,
			ILogger<FiscalDataService> logger
		)
		{
			_dataContext = dataContext;
			_dateTimeProvider = dateTimeProvider;
			_logger = logger;
		}

		public async Task<Receipt> GetReceipt(QRCode qrcode)
		{
			var organization = _dataContext.Organizations[Random.Next(_dataContext.Organizations.Count)];
			var receipt = new Receipt
			{
				FiscalDocumentNumber = qrcode.FiscalDocumentNumber,
				FiscalDriveNumber = qrcode.FiscalDriveNumber,
				FiscalType = qrcode.FiscalType,
				DateTime = DateTime.TryParse(qrcode.DateTime, out var dateTime) ? dateTime : _dateTimeProvider.Now.AddDays(-1),
				TotalSum = Decimal.TryParse(qrcode.TotalSum, out var totalSum) ? totalSum : 0,
				OperationType = 1,
				Place = "Место совершения покупки",
				OrganizationID = organization.ID,
				Organization = organization
			};
			receipt.ProductItems = GetProductItems(receipt);

			return await Task.FromResult(receipt);
		}

		public List<ProductItem> GetProductItems(Receipt receipt)
		{
			var productItems = _dataContext.ProductItems
				.OrderBy(pi => Random.Next())
				.ToArray();
			
			var result = new List<ProductItem>();

			var resultSum = 0;
			foreach (var productItem in productItems)
			{
				var daysMultiplier = 0.001m;
				var daysCount = (Decimal)(receipt.DateTime - productItem.Receipt.DateTime).TotalDays;
				var price = daysCount > 0 ? productItem.Price * daysCount * (1 + daysMultiplier) : productItem.Price;
				var sum = productItem.Quantity * price;

				if (resultSum + sum <= receipt.TotalSum)
				{
					result.Add(new ProductItem
					{
						Name = productItem.Name,
						Price = price,
						Quantity = productItem.Quantity,
						Sum = sum
					});
				}
				else
				{
					result.Add(new ProductItem
					{
						Name = "На кофе разработчику",
						Price = receipt.TotalSum - resultSum,
						Quantity = 1.00m,
						Sum = receipt.TotalSum - resultSum
					});

					break;
				}
			}

			return result
				.GroupBy(pi => new { pi.Name, pi.Price })
				.Select(g => new ProductItem
				{
					Name = g.Key.Name,
					Price = g.Key.Price,
					Quantity = g.Sum(pi => pi.Quantity),
					Sum = g.Sum(pi => pi.Sum),
					Receipt = receipt
				})
				.ToList();
		}
	}
}
