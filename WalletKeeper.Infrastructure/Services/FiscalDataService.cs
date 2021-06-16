using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WalletKeeper.Domain.Configs;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Exceptions;
using WalletKeeper.Domain.Services;
using WalletKeeper.Domain.Types;

namespace WalletKeeper.Infrastructure.Services
{
	public class FiscalDataService : IFiscalDataService
	{
		private readonly HttpClient _httpClient;
		private readonly FiscalDataServiceConfig _config;

		private readonly ILogger<FiscalDataService> _logger;

		public FiscalDataService(
			HttpClient httpClient,
			IOptions<FiscalDataServiceConfig> configOptions,
			ILogger<FiscalDataService> logger
		)
		{
			_httpClient = httpClient;
			_config = configOptions.Value;
			_logger = logger;
		}

		public async Task<Receipt> GetReceipt(QRCode qrcode)
		{
			var request = new Dictionary<String, String>
			{
				["fn"] = qrcode.FiscalDriveNumber,
				["fd"] = qrcode.FiscalDocumentNumber,
				["fp"] = qrcode.FiscalType,
				["t"] = qrcode.DateTime,
				["n"] = qrcode.OperationType,
				["s"] = qrcode.TotalSum,
				["qr"] = "1",
				["token"] = _config.Token
			};

			var requestContent = new FormUrlEncodedContent(request);

			var response = await _httpClient.PostAsync("/api/v1/check/get", requestContent);

			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();

			var receipt = Parse(responseContent);

			return receipt;
		}

		private static Receipt Parse(String jsonString)
		{
			var code = JObject.Parse(jsonString).Value<Int32>("code");

			if (code == 1)
			{
				var jObject = JObject.Parse(jsonString)["data"]["json"];

				var receipt = new Receipt
				{
					FiscalDocumentNumber = jObject.Value<String>("fiscalDocumentNumber"),
					FiscalDriveNumber = jObject.Value<String>("fiscalDriveNumber"),
					FiscalType = jObject.Value<String>("fiscalSign"),
					DateTime = jObject.Value<DateTime>("dateTime"),
					TotalSum = jObject.Value<Decimal>("totalSum") / 100,
					OperationType = jObject.Value<Int32>("operationType"),
					Place = jObject.Value<String>("retailPlace"),
					Organization = new Organization
					{
						INN = jObject.Value<String>("userInn").Trim(),
						Name = jObject.Value<String>("user"),
					}
				};

				receipt.ProductItems = jObject["items"]
					.GroupBy(jt => new { Name = jt.Value<String>("name"), Price = jt.Value<Decimal>("price") / 100, VAT = jt.Value<Decimal>("nds") / 10 })
					.Select(g => new ProductItem
					{
						Name = g.Key.Name,
						Price = g.Key.Price,
						VAT = g.Key.VAT,
						Quantity = g.Sum(jt => jt.Value<Decimal>("quantity")),
						Sum = g.Sum(jt => jt.Value<Decimal>("sum")) / 100,
						Receipt = receipt
					})
					.ToList();

				return receipt;
			}
			else
			{
				var message = JObject.Parse(jsonString).Value<String>("data");

				throw new BusinessException(message);
			}
		}
	}
}
