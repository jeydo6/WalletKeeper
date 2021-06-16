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
using WalletKeeper.Barcodes.Encoders;
using WalletKeeper.Domain.Configs;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Factories;
using WalletKeeper.Domain.Providers;
using WalletKeeper.Domain.Repositories;
using WalletKeeper.Domain.Services;
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
			services
				.AddControllers(options =>
				{
					options.Filters.Add<ExceptionFilter>();
					options.Filters.Add<BusinessExceptionFilter>();
					options.Filters.Add<ValidationExceptionFilter>();
				})
				.AddNewtonsoftJson();

			services
				.AddSpaStaticFiles(options =>
				{
					options.RootPath = "wwwroot";
				});

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
				.AddSingleton<MagickQRCodeDecoder>()
				.AddSingleton<MagickQRCodeEncoder>()
				.AddSingleton<EmailMessageFactory>();

			#region ConfigureServices
#if DEMO
			services
				.ConfigureDemoServices();
#elif DEBUG
			services
				.ConfigureLiveServices(options =>
				{
					options.ConnectionString = Configuration
						.GetConnectionString("DbConnection");

					options.AuthenticationConfig = Configuration
						.GetSection($"{nameof(AuthenticationConfig)}")
						.Get<AuthenticationConfig>();
				});
#elif RELEASE
			services
				.ConfigureLiveServices(options =>
				{
					options.ConnectionString = Configuration
						.GetConnectionString("DbConnection");

					options.AuthenticationConfig = Configuration
						.GetSection($"{nameof(AuthenticationConfig)}")
						.Get<AuthenticationConfig>();
				});
#else
			throw new Exception("Unknown project configuration!");
#endif
			#endregion

			services
				.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
				{
					var authenticationConfig = Configuration
						.GetSection($"{nameof(AuthenticationConfig)}")
						.Get<AuthenticationConfig>();

					options.RequireHttpsMetadata = true;
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidIssuer = authenticationConfig.Issuer,
						ValidateAudience = false,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationConfig.Secret))
					};
				});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseCors(builder => builder
				.AllowAnyHeader()
				.AllowAnyMethod()
				.AllowAnyOrigin()
			);

			app.UseHttpsRedirection();
			app.UseStaticFiles();

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
				options.RoutePrefix = "api/swagger";
				options.SwaggerEndpoint("/swagger/main/swagger.json", "WalletKeeperAPI");
			});

			if (env.IsDevelopment())
			{
				app.UseSpa(spa =>
				{
					spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
				});
			}
			else
			{
				app.UseSpaStaticFiles();
				app.UseSpa(spa =>
				{
					spa.Options.SourcePath = "wwwroot";
				});
			}
		}
	}

	internal static class StartupExtension
	{
		public static void ConfigureLiveServices(this IServiceCollection services, Action<LiveServicesOptions> configureOptions)
		{
			var liveOptions = new LiveServicesOptions();
			configureOptions(liveOptions);

			services
				.AddDbContext<Persistence.DbContexts.ApplicationDbContext>(options =>
					options.UseSqlServer(liveOptions.ConnectionString)
				);

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
				.AddEntityFrameworkStores<Persistence.DbContexts.ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services
				.AddHttpClient<IFiscalDataService, Infrastructure.Services.FiscalDataService>((sp, httpClient) =>
				{
					var configOptions = sp.GetRequiredService<IOptions<FiscalDataServiceConfig>>();
					var config = configOptions.Value;

					httpClient.BaseAddress = new Uri(config.Address);
				});

			services
				.AddSingleton<IEmailService, Infrastructure.Services.EmailService>();

			services
				.AddSingleton<IDateTimeProvider, CurrentDateTimeProvider>();

			services
				.AddScoped<IPrincipal, ClaimsPrincipal>(sp =>
				{
					var httpContextAccessor = sp.GetService<IHttpContextAccessor>();

					return httpContextAccessor.HttpContext.User;
				});

			services
				.AddScoped<ICategoriesRepository, Persistence.Repositories.CategoriesRepository>()
				.AddScoped<IProductsRepository, Persistence.Repositories.ProductsRepository>()
				.AddScoped<IProductItemsRepository, Persistence.Repositories.ProductItemsRepository>()
				.AddScoped<IReceiptsRepository, Persistence.Repositories.ReceiptsRepository>()
				.AddScoped<IUsersRepository, Persistence.Repositories.UsersRepository>();
		}

		public static void ConfigureDemoServices(this IServiceCollection services)
		{
			services
				.AddSingleton<IEmailService, Demo.Services.EmailService>()
				.AddSingleton<IFiscalDataService, Demo.Services.FiscalDataService>();

			services
				.AddSingleton<IDateTimeProvider, CurrentDateTimeProvider>();

			services
				.AddHttpContextAccessor()
				.AddScoped<IPrincipal, ClaimsPrincipal>(sp =>
				{
					var httpContextAccessor = sp.GetService<IHttpContextAccessor>();

					return httpContextAccessor.HttpContext.User;
				})
				.AddScoped<IUserClaimsPrincipalFactory<User>, Demo.Factories.UserClaimsPrincipalFactory>();

			services
				.AddSingleton<Demo.DataContexts.ApplicationDataContext>();

			services
				.AddScoped<ICategoriesRepository, Demo.Repositories.CategoriesRepository>()
				.AddScoped<IProductsRepository, Demo.Repositories.ProductsRepository>()
				.AddScoped<IProductItemsRepository, Demo.Repositories.ProductItemsRepository>()
				.AddScoped<IReceiptsRepository, Demo.Repositories.ReceiptsRepository>()
				.AddScoped<IUsersRepository, Demo.Repositories.UsersRepository>();
		}

	}
}
