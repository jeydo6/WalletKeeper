using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
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
		private static readonly NumberFormatInfo DotDecimalSeparator = new()
		{
			NumberDecimalSeparator = "."
		};

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
				DateTime = DateTime.TryParseExact(qrcode.DateTime, "yyyyMMddTHHmm", CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateTime) ? dateTime : _dateTimeProvider.Now.AddDays(-1),
				TotalSum = Decimal.TryParse(qrcode.TotalSum, NumberStyles.Float, DotDecimalSeparator, out var totalSum) ? totalSum : 0,
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

			var resultSum = 0.00m;
			foreach (var productItem in productItems)
			{
				var daysMultiplier = 0.0005;
				var daysCount = (receipt.DateTime - productItem.Receipt.DateTime).TotalDays;

				var priceMultiplier = daysCount > 0 ? (Decimal)Math.Pow(1 + daysMultiplier, daysCount) : 1;

				var price = Math.Round(productItem.Price * priceMultiplier, 2);
				var sum = Math.Round(productItem.Quantity * price, 2);

				if (resultSum + sum > receipt.TotalSum)
				{
					break;
				}

				result.Add(new ProductItem
				{
					Name = productItem.Name,
					Price = price,
					Quantity = productItem.Quantity,
					Sum = sum,
					VAT = 0.10m
				});

				resultSum += sum;
			}

			if (resultSum < receipt.TotalSum)
			{
				result.Add(new ProductItem
				{
					Name = "На кофе разработчику",
					Price = receipt.TotalSum - resultSum,
					Quantity = 1.0000m,
					Sum = receipt.TotalSum - resultSum,
					VAT = 0.00m
				});
			}

			return result
				.GroupBy(pi => new { pi.Name, pi.Price })
				.Select(g => new ProductItem
				{
					Name = g.Key.Name,
					Price = g.Key.Price,
					Quantity = g.Sum(pi => pi.Quantity),
					Sum = g.Sum(pi => pi.Sum),
					Receipt = receipt,
					VAT = 0.10m
				})
				.ToList();
		}
	}
}
