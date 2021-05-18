using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using WalletKeeper.Barcodes.Decoders;
using WalletKeeper.Demo.DataContexts;
using WalletKeeper.Domain.Configs;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Factories;
using WalletKeeper.Domain.Providers;
using WalletKeeper.Domain.Repositories;
using WalletKeeper.Domain.Services;
using WalletKeeper.Infrastructure.Services;
using WalletKeeper.Persistence.DbContexts;
using WalletKeeper.Persistence.Factories;
using WalletKeeper.Persistence.Repositories;
using WalletKeeper.WebAPI.Filters;
using WalletKeeper.WebAPI.Options;

namespace WalletKeeper.WebAPI
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			var test = new ApplicationDataContext(new CurrentDateTimeProvider());

			services
				.AddControllers(options =>
				{
					options.Filters.Add<ExceptionFilter>();
					options.Filters.Add<BusinessExceptionFilter>();
					options.Filters.Add<ValidationExceptionFilter>();
				})
				.AddNewtonsoftJson();

			services
				.AddOptions();

			services
				.AddMediatR(typeof(Application.AssemblyMarker));

			services
				.AddSwaggerGen(options =>
				{
					options.SwaggerDoc("main", new OpenApiInfo
					{
						Version = System.Reflection.Assembly
							.GetEntryAssembly()
							.GetName()
							.Version
							.ToString(),
						Title = "WalletKeeperAPI",
						Description = "Wallet Keeper WebAPI"
					});

					options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
					{
						In = ParameterLocation.Header,
						Description = "Please enter into field the word 'Bearer' following by space and JWT",
						Name = "Authorization",
						Type = SecuritySchemeType.ApiKey
					});

					options.AddSecurityRequirement(new OpenApiSecurityRequirement
					{
						{
							new OpenApiSecurityScheme
							{
								In = ParameterLocation.Header,
								Name = JwtBearerDefaults.AuthenticationScheme,
								Scheme = "oauth2",
								Reference = new OpenApiReference
								{
									Type = ReferenceType.SecurityScheme,
									Id = JwtBearerDefaults.AuthenticationScheme
								}
							},
							new List<String>()
						}
					});
				});

			services
				.Configure<AuthenticationConfig>(Configuration.GetSection($"{nameof(AuthenticationConfig)}"))
				.Configure<FiscalDataServiceConfig>(Configuration.GetSection($"{nameof(FiscalDataServiceConfig)}"))
				.Configure<SmtpConfig>(Configuration.GetSection($"{nameof(SmtpConfig)}"));

			services
				.AddDbContext<ApplicationDbContext>(options =>
					options.UseSqlServer(Configuration.GetConnectionString("DbConnection"))
				);

			services
				.AddAuthentication(options =>
				{
					options.AuthenticationConfig = Configuration
						.GetSection($"{nameof(AuthenticationConfig)}")
						.Get<AuthenticationConfig>();
				});

			services
				.AddHttpClient<IFiscalDataService, FiscalDataService>((sp, httpClient) =>
				{
					var configOptions = sp.GetRequiredService<IOptions<FiscalDataServiceConfig>>();
					var config = configOptions.Value;

					httpClient.BaseAddress = new Uri(config.Address);
				});

			services
				.AddSingleton<MagickQRCodeDecoder>()
				.AddSingleton<EmailMessageFactory>();

			services
				.AddSingleton<IEmailService, EmailService>();

			services
				.AddSingleton<IDateTimeProvider, CurrentDateTimeProvider>();

			services
				.AddScoped<IPrincipal, ClaimsPrincipal>(sp =>
				{
					var httpContextAccessor = sp.GetService<IHttpContextAccessor>();

					return httpContextAccessor.HttpContext.User;
				})
				.AddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory>();

			services
				.AddScoped<ICategoriesRepository, CategoriesRepository>()
				.AddScoped<IProductsRepository, ProductsRepository>()
				.AddScoped<IProductItemsRepository, ProductItemsRepository>()
				.AddScoped<IReceiptsRepository, ReceiptsRepository>()
				.AddScoped<IUsersRepository, UsersRepository>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseCors(builder => builder
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowAnyOrigin()
			);

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.RoutePrefix = "";
				options.SwaggerEndpoint("/swagger/main/swagger.json", "WalletKeeperAPI");
			});
		}
	}

	internal static class StartupExtension
	{
		public static void AddAuthentication(this IServiceCollection services, Action<AuthenticationOptions> configureOptions)
		{
			var options = new AuthenticationOptions();
			configureOptions(options);

			var config = options.AuthenticationConfig;

			services
				.AddIdentity<User, Role>(options =>
				{
					options.SignIn = new SignInOptions
					{
						RequireConfirmedEmail = false
					};

					options.Password = new PasswordOptions
					{
						RequiredLength = 6,
						RequireDigit = false,
						RequireNonAlphanumeric = false,
						RequireUppercase = false,
						RequireLowercase = false
					};

					options.User = new UserOptions
					{
						RequireUniqueEmail = true
					};

					options.Lockout = new LockoutOptions
					{
						DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5)
					};
				})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services
				.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
				{
					options.RequireHttpsMetadata = true;
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidIssuer = config.Issuer,
						ValidateAudience = false,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret))
					};
				});
		}
	}
}
