using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
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

			IEnumerable<IMutableForeignKey> foreignKeys = modelBuilder.Model
				.GetEntityTypes()
				.SelectMany(et => et.GetForeignKeys());

			foreach (IMutableForeignKey foreignKey in foreignKeys)
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
		}
	}
}
