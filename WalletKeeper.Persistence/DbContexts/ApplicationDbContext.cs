using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WalletKeeper.Domain.Entities;

namespace WalletKeeper.Persistence.DbContexts
{
	public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
	{
		public ApplicationDbContext(
			DbContextOptions<ApplicationDbContext> options
		) : base(options)
		{
			//
		}

		public DbSet<Organization> Organizations { get; set; }

		public DbSet<Category> Categories { get; set; }

		public DbSet<Product> Products { get; set; }

		public DbSet<ProductItem> ProductItems { get; set; }

		public DbSet<Receipt> Receipts { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			var foreignKeys = modelBuilder.Model
				.GetEntityTypes()
				.SelectMany(et => et.GetForeignKeys());

			foreach (var foreignKey in foreignKeys)
			{
				foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
			}

			modelBuilder.Entity<User>()
				.ToTable("Users");

			modelBuilder.Entity<Role>()
				.ToTable("Roles");

			modelBuilder.Entity<IdentityUserLogin<Guid>>()
				.ToTable("UserLogins");

			modelBuilder.Entity<IdentityUserClaim<Guid>>()
				.ToTable("UserClaims");

			modelBuilder.Entity<IdentityUserToken<Guid>>()
				.ToTable("UserTokens");

			modelBuilder.Entity<IdentityUserRole<Guid>>()
				.ToTable("UserRoles");

			modelBuilder.Entity<IdentityRoleClaim<Guid>>()
				.ToTable("RoleClaims");

			modelBuilder.Entity<Category>()
				.ToTable("Categories")
				.HasKey(e => e.ID);

			modelBuilder.Entity<Organization>()
				.ToTable("Organizations")
				.HasKey(e => e.ID);

			modelBuilder.Entity<Organization>()
				.HasIndex(e => e.INN)
				.IsUnique(true);

			modelBuilder.Entity<Product>()
				.ToTable("Products")
				.HasKey(e => e.ID);

			modelBuilder.Entity<ProductItem>()
				.ToTable("ProductItems")
				.HasKey(e => e.ID);

			modelBuilder.Entity<ProductItem>()
				.Property(e => e.Price)
				.HasPrecision(18, 2);

			modelBuilder.Entity<ProductItem>()
				.Property(e => e.Quantity)
				.HasPrecision(18, 4);

			modelBuilder.Entity<ProductItem>()
				.Property(e => e.Sum)
				.HasPrecision(18, 2);

			modelBuilder.Entity<ProductItem>()
				.Property(e => e.NDS)
				.HasPrecision(18, 2);

			modelBuilder.Entity<Receipt>()
				.ToTable("Receipts")
				.HasKey(e => e.ID);

			modelBuilder.Entity<Receipt>()
				.HasIndex(e => new { e.FiscalDocumentNumber, e.FiscalDriveNumber })
				.IsUnique(true);

			modelBuilder.Entity<Receipt>()
				.Property(e => e.TotalSum)
				.HasPrecision(18, 2);
		}
	}
}
