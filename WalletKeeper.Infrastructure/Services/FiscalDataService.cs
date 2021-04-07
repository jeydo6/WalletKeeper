using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Services;

namespace WalletKeeper.Infrastructure.Services
{
	public class FiscalDataService : IFiscalDataService
	{
		private static readonly String _responseContent = "{\"code\":1,\"data\":{\"json\":{\"code\":3,\"user\":\"ОБЩЕСТВО С ОГРАНИЧЕННОЙ ОТВЕТСТВЕННОСТЬЮ \\\"АШАН\\\"\",\"items\":[{\"nds\":1,\"sum\":65388,\"name\":\"РЫЧАЛ-СУ ЛЕЧ-СТОЛ.ГАЗ 0.5 Л.\",\"price\":5449,\"quantity\":12,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":5439,\"name\":\"КАРТОФЕЛЬ ЕГИПЕТ ВЕС\",\"price\":4990,\"quantity\":1.09,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":6451,\"name\":\"КАБАЧКИ КГ ВЕС\",\"price\":11990,\"quantity\":0.538,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":5947,\"name\":\"1АПУСТА БРОККОЛИ ВЕС\",\"price\":16990,\"quantity\":0.35,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":8935,\"name\":\"БАНАНЫ ВЕС\",\"price\":5949,\"quantity\":1.502,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":9500,\"name\":\"КП\\/БЛИНЫ С ЯБЛОКОМ\",\"price\":9500,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":9249,\"name\":\"GM БЛИНЧИКИ С МЯСОМ 420ГР\",\"price\":9249,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":2999,\"name\":\"АССОРТИ УКР\\/ПЕТР 30ГР ЛОТОК\",\"price\":2999,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":10490,\"name\":\"РЫБ.ПОРЦИИ ALASKA POLLOCK,145\",\"price\":10490,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":5725,\"name\":\"ПЕРЕЦ СЛАДКИЙ РАМИРО ВЕС\",\"price\":22900,\"quantity\":0.25,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":6990,\"name\":\"ШНИЦЕЛЬ 300Г\",\"price\":6990,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":11099,\"name\":\"СЕН-СОЙ СОУС КЛАССИЧ\",\"price\":11099,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":13449,\"name\":\"GM МАСЛО РАФ С ОЛИВ 0.81 АЛЬТ\",\"price\":13449,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":18099,\"name\":\"УП ОРЕХ ГРЕЦКИЙ № 1 Б\\/К 150ГР\",\"price\":18099,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":18323,\"name\":\"ГРУДКА БЕЗ КОЖИ ПЕТЕЛИНКА ОХЛ\",\"price\":22990,\"quantity\":0.797,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":19999,\"name\":\"ТОМАТ ЧЕРРИ НА ВЕТКЕ 250Г\",\"price\":19999,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":7990,\"name\":\"GM СЕМЕЧК МАРТИНА МОР СОЛЬ200Г\",\"price\":7990,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":7990,\"name\":\"GM СЕМЕЧК МАРТИНА МОР СОЛЬ200Г\",\"price\":7990,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":7990,\"name\":\"GM СЕМЕЧК МАРТИНА МОР СОЛЬ200Г\",\"price\":7990,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":1800,\"name\":\"\\\"\\\"БАГЕТ ПШЕНИЧНЫЙ 250Г\",\"price\":1800,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":25049,\"name\":\"КОЛБ.ДОКТОРСКАЯ ГОСТ П\\/О 0.5КГ\",\"price\":25049,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":11990,\"name\":\"GM СЛИВКИ 10% 0.75Л ДВД\",\"price\":11990,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":7590,\"name\":\"КП \\/КУСКУС  500ГР\",\"price\":7590,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":12090,\"name\":\"НОРДИК 4-Х ЗЕРН.ХЛОПЬЯ 600Г\",\"price\":12090,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":7499,\"name\":\"МОЛОКО 3.2% 0.93КГ ПЭТ СЕЛ ЗЕЛ\",\"price\":7499,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":2099,\"name\":\"СЫРОК ИЗЮМ 23%100Г\",\"price\":2099,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":2099,\"name\":\"СЫРОК ИЗЮМ 23%100Г\",\"price\":2099,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":4490,\"name\":\"АЛЬП. ГОЛЬД С АРАХ И КРЕКЕР85Г\",\"price\":4490,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":13470,\"name\":\"GM АЛЬПЕН ГОЛЬД ОРЕО 19Х95 Г\",\"price\":4490,\"quantity\":3,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":17196,\"name\":\"СЫР ТВ КОКОС БЕЛ ГЛ 23%45БЗМЖ\",\"price\":4299,\"quantity\":4,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":6396,\"name\":\"GM КАМП.НЕЖН.С СОК. КЛУБН 100Г\",\"price\":1599,\"quantity\":4,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":5299,\"name\":\"ЙОГ ТЕРМ КОКОС ШОК\",\"price\":5299,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":5299,\"name\":\"ЙОГ ТЕРМ КОКОС ШОК\",\"price\":5299,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":3990,\"name\":\"МАГГИ НА ВТ. СОЧН ИНДЕЙКА 30Г\",\"price\":3990,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":6799,\"name\":\"ЙОГ ГР-ВАН ГР ОР5,3% 190 БЗМЖ\",\"price\":6799,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":4999,\"name\":\"ЙОГ КЛУБНИКА 4.8% 130Г EPICA\",\"price\":4999,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":6799,\"name\":\"ЙОГ ЯБЛ\\/КОР 4.8%190Г\",\"price\":6799,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":2,\"sum\":5299,\"name\":\"ЙОГ КИВИ-ФЕЙХОА 4,8% 130Г БЗМЖ\",\"price\":5299,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":4449,\"name\":\"КР-МЫЛ ОВС\\/500 Д-ПАК ОС\",\"price\":4449,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":6990,\"name\":\"З\\/Щ BIOMED SILVER\",\"price\":6990,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":6990,\"name\":\"З\\/Щ BIOMED SILVER\",\"price\":6990,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":22899,\"name\":\"КР Д\\/РУК ВОС СУХ.К 100М ГАРНЬЕ\",\"price\":22899,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":3601,\"name\":\"ГУБК\\/Д\\/ПОС\\/АГАВЫ 2ШТ\",\"price\":3601,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":3601,\"name\":\"ГУБК\\/Д\\/ПОС\\/АГАВЫ 2ШТ\",\"price\":3601,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":30599,\"name\":\"СИЛИТ БЭНГ ОЧИСТ.КУРОК 750МЛ\",\"price\":30599,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":11990,\"name\":\"КОНД Д ДЕТ БЕЛ 910 МЛ ВЕРНЕЛЬ\",\"price\":11990,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":11990,\"name\":\"КОНД Д ДЕТ БЕЛ 910 МЛ ВЕРНЕЛЬ\",\"price\":11990,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":11949,\"name\":\"МЕШКИ С ЗАВЯЗ 60Л ЭКОНОМ 30ШТ\\/\",\"price\":11949,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":26900,\"name\":\"GM ПОР ДЕТ 2.4КГ УШАСТЫЙ НЯНЬ\",\"price\":26900,\"quantity\":1,\"paymentType\":4,\"productType\":1},{\"nds\":1,\"sum\":9399,\"name\":\"OILRIGHT ВОДА ДИСТ 5Л\",\"price\":9399,\"quantity\":1,\"paymentType\":4,\"productType\":1}],\"nds10\":24257,\"nds18\":46139,\"region\":\"50\",\"userInn\":\"7703270067  \",\"dateTime\":\"2021-03-14T12:01:00\",\"kktRegId\":\"0000393487044552    \",\"metadata\":{\"id\":3619057295181063425,\"ofdId\":\"ofd1\",\"address\":\"140055,РОССИЯ,Московская область, ,Котельники г г.п., , ,1-й Покровский проезд элем. улично-дорожн.сети,,д. 5, , , , \",\"subtype\":\"receipt\",\"receiveDate\":\"2021-03-14T09:03:28Z\"},\"operator\":\"Федосова Нина\",\"totalSum\":543661,\"creditSum\":0,\"numberKkt\":\"00106903432874\",\"fiscalSign\":837643721,\"prepaidSum\":0,\"retailPlace\":\"АШАН Белая Дача\",\"shiftNumber\":122,\"cashTotalSum\":0,\"provisionSum\":0,\"ecashTotalSum\":543661,\"operationType\":1,\"redefine_mask\":10,\"requestNumber\":17,\"fiscalDriveNumber\":\"9282440300732022\",\"messageFiscalSign\":9.297202187957266e+18,\"appliedTaxationType\":1,\"fiscalDocumentNumber\":20466,\"fiscalDocumentFormatVer\":2},\"html\":\"\"}}";

		public async Task<Receipt> GetReceipt(ReceiptHeader receiptHeader)
		{
			var jsonData = JObject.Parse(_responseContent)["data"]["json"];

			var receipt = new Receipt
			{
				FiscalDocumentNumber = jsonData.Value<String>("fiscalDocumentNumber"),
				FiscalDriveNumber = jsonData.Value<String>("fiscalDriveNumber"),
				FiscalType = jsonData.Value<String>("fiscalSign"),
				DateTime = jsonData.Value<DateTime>("dateTime"),
				TotalSum = jsonData.Value<Decimal>("totalSum") / 100,
				OperationType = jsonData.Value<Int32>("operationType"),
				Place = jsonData.Value<String>("retailPlace"),
				Organization = new Organization
				{
					INN = jsonData.Value<String>("userInn").Trim(),
					Name = jsonData.Value<String>("user"),
				}
			};

			receipt.Products = jsonData["items"]
				.Select(jt => new Product
				{
					Name = jt.Value<String>("name"),
					Price = jt.Value<Decimal>("price") / 100,
					Quantity = jt.Value<Decimal>("quantity"),
					Sum = jt.Value<Decimal>("sum") / 100,
					Receipt = receipt
				})
				.ToList();

			return await Task.FromResult(receipt);
		}
	}
}
