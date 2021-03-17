using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using WalletKeeper.Barcodes.Decoders;
using WalletKeeper.Domain.Entities;
using WalletKeeper.Domain.Factories;
using WalletKeeper.Persistence.DbContexts;
using WalletKeeper.WebAPI.Configs;

namespace WalletKeeper.WebAPI
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(Configuration)
				.CreateLogger();

			EndpointConfig = Configuration
				.GetSection("EndpointConfig")
				.Get<EndpointConfig>();
		}

		public IConfiguration Configuration { get; }

		private EndpointConfig EndpointConfig { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddControllers()
				.AddNewtonsoftJson();

			services
				.AddOptions();

			services
				.Configure<EndpointConfig>(
					Configuration.GetSection("EndpointConfig")
				);

			services
				.AddDbContext<ApplicationDbContext>(options =>
					options.UseSqlServer(
						Configuration.GetConnectionString("DbConnection")
					)
				);

			services
				.AddSingleton<IBarcodeDecoder, MagickBarcodeDecoder>();

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

			AddAuthentication(services);
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

		public void AddAuthentication(IServiceCollection services)
		{
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
				.AddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory>();

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
						ValidIssuer = EndpointConfig.Issuer,
						ValidateAudience = false,
						//ValidAudiences = EndpointConfig.Audiences,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EndpointConfig.Secret))
					};
				});
		}
	}
}
