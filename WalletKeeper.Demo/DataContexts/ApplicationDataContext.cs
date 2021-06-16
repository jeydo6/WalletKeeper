using System;
using System.Collections.Generic;
using System.Linq;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Providers;

namespace WalletKeeper.Demo.DataContexts
{
	public class ApplicationDataContext
	{
		private readonly IDateTimeProvider _dateTimeProvider;

		public ApplicationDataContext(
			IDateTimeProvider dateTimeProvider
		)
		{
			_dateTimeProvider = dateTimeProvider;

			Organizations = new List<Organization>();
			Categories = new List<Category>();
			Products = new List<Product>();
			ProductItems = new List<ProductItem>();
			Receipts = new List<Receipt>();
			Users = new List<User>();
			Roles = new List<Role>();
			UserRoles = new List<(Guid UserId, Guid RoleId)>();

			Seed();
		}

		public List<Organization> Organizations { get; }

		public List<Category> Categories { get; }

		public List<Product> Products { get; }

		public List<ProductItem> ProductItems { get; }

		public List<Receipt> Receipts { get; }

		public List<User> Users { get; }

		public List<Role> Roles { get; }

		internal List<(Guid UserId, Guid RoleId)> UserRoles { get; }

		private void Seed()
		{
			var role = new Role
			{
				Id = new Guid("00000000-0000-0000-0000-000000000001"),
				Level = 2,
				Name = "User",
				NormalizedName = "USER"
			};
			Roles.Add(role);

			var user = new User
			{
				Id = new Guid("00000000-0000-0000-0000-000000000001"),
				UserName = "User",
				NormalizedUserName = "USER",
				Email = "User@builtin.local",
				NormalizedEmail = "USER@BUILTIN.LOCAL",
				EmailConfirmed = false,
				PasswordHash = "123456",
				PhoneNumberConfirmed = false,
				TwoFactorEnabled = false,
				LockoutEnabled = true,
				AccessFailedCount = 0
			};
			Users.Add(user);

			var userRole = (user.Id, role.Id);
			UserRoles.Add(userRole);

			var organization = new Organization
			{
				ID = 1,
				INN = "1234567890",
				Name = "Продуктовый магазин № 1"
			};
			Organizations.Add(organization);

			var category1 = new Category
			{
				ID = 1,
				Name = "Овощи",
				Products = new List<Product>
				{
					new Product
					{
						ID = 1,
						Name = "Картофель"
					},
					new Product
					{
						ID = 2,
						Name = "Томаты"
					},
					new Product
					{
						ID = 3,
						Name = "Огурцы"
					},
					new Product
					{
						ID = 4,
						Name = "Болгарский перец"
					},
					new Product
					{
						ID = 5,
						Name = "Кабачки"
					}
				}
			};
			foreach (var product in category1.Products)
			{
				product.CategoryID = category1.ID;
				product.Category = category1;
				product.UserID = user.Id;
				product.User = user;
			}
			user.Products.AddRange(category1.Products);
			Categories.Add(category1);
			Products.AddRange(category1.Products);

			var category2 = new Category
			{
				ID = 2,
				Name = "Фрукты",
				Products = new List<Product>
				{
					new Product
					{
						ID = 6,
						Name = "Яблоки"
					},
					new Product
					{
						ID = 7,
						Name = "Груши"
					},
					new Product
					{
						ID = 8,
						Name = "Апельсины"
					},
					new Product
					{
						ID = 9,
						Name = "Киви"
					},
					new Product
					{
						ID = 10,
						Name = "Манго"
					}
				}
			};
			foreach (var product in category2.Products)
			{
				product.CategoryID = category2.ID;
				product.Category = category2;
				product.UserID = user.Id;
				product.User = user;
			}
			user.Products.AddRange(category2.Products);
			Categories.Add(category2);
			Products.AddRange(category2.Products);

			var category3 = new Category
			{
				ID = 3,
				Name = "Пожертвования",
				Products = new List<Product>
				{
					new Product
					{
						ID = 11,
						Name = "На кофе разработчику"
					}
				}
			};
			foreach (var product in category3.Products)
			{
				product.CategoryID = category3.ID;
				product.Category = category3;
				product.UserID = user.Id;
				product.User = user;
			}
			user.Products.AddRange(category3.Products);
			Categories.Add(category3);
			Products.AddRange(category3.Products);

			var receipt = new Receipt
			{
				ID = 1,
				FiscalDocumentNumber = "123456",
				FiscalDriveNumber = "1234567812345678",
				FiscalType = "1234",
				DateTime = _dateTimeProvider.Now.AddDays(-14),
				OperationType = 1,
				Place = "Место совершения покупки",
				OrganizationID = organization.ID,
				Organization = organization,
				UserID = user.Id,
				User = user,
				ProductItems = new List<ProductItem>
				{
					new ProductItem
					{
						ID = 1,
						Name = "Картофель",
						Price = 60.00m,
						Quantity = 1.00m,
						NDS = 0.10m
					},
					new ProductItem
					{
						ID = 2,
						Name = "Томаты",
						Price = 300.00m,
						Quantity = 0.50m,
						NDS = 0.10m
					},
					new ProductItem
					{
						ID = 3,
						Name = "Огурцы",
						Price = 160.00m,
						Quantity = 1.00m,
						NDS = 0.10m
					},
					new ProductItem
					{
						ID = 4,
						Name = "Болгарский перец",
						Price = 360.00m,
						Quantity = 0.30m,
						NDS = 0.10m
					},
					new ProductItem
					{
						ID = 5,
						Name = "Кабачки",
						Price = 90.00m,
						Quantity = 1.00m,
						NDS = 0.10m
					},
					new ProductItem
					{
						ID = 6,
						Name = "Яблоки",
						Price = 180.00m,
						Quantity = 2.00m,
						NDS = 0.10m
					},
					new ProductItem
					{
						ID = 7,
						Name = "Груши",
						Price = 210.00m,
						Quantity = 1.50m,
						NDS = 0.10m
					},
					new ProductItem
					{
						ID = 8,
						Name = "Апельсины",
						Price = 140.00m,
						Quantity = 0.50m,
						NDS = 0.10m
					},
					new ProductItem
					{
						ID = 9,
						Name = "Киви",
						Price = 300.00m,
						Quantity = 0.30m,
						NDS = 0.10m
					},
					new ProductItem
					{
						ID = 10,
						Name = "Манго",
						Price = 660.00m,
						Quantity = 0.50m,
						NDS = 0.10m
					}
				}
			};
			receipt.TotalSum = receipt.ProductItems
				.Select(pi =>
				{
					pi.Sum = Math.Round(pi.Quantity * pi.Price, 2);

					return pi;
				})
				.Sum(pi => pi.Sum);
			foreach (var productItem in receipt.ProductItems)
			{
				productItem.ReceiptID = receipt.ID;
				productItem.Receipt = receipt;

				var product = Products.FirstOrDefault(p => p.Name == productItem.Name);
				if (product != null)
				{
					productItem.ProductID = product.ID;
					productItem.Product = product;
					productItem.UserID = user.Id;
					productItem.User = user;

					product.ProductItems.Add(productItem);
				}
			}
			user.ProductItems.AddRange(receipt.ProductItems);
			Receipts.Add(receipt);
			ProductItems.AddRange(receipt.ProductItems);
		}
	}
}
