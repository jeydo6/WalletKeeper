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
	public class IdentityDbContext : IdentityDbContext<User, Role, Guid>
	{
		public IdentityDbContext(
			DbContextOptions<IdentityDbContext> options
		) : base(options)
		{
			//
		}

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
