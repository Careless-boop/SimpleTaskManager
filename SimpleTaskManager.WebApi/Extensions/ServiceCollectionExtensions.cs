using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SimpleTaskManager.BLL.Configurations;
using SimpleTaskManager.BLL.Interfaces;
using SimpleTaskManager.BLL.Services;
using SimpleTaskManager.DAL;
using SimpleTaskManager.DAL.Repository.Interfaces;
using SimpleTaskManager.DAL.Repository.Realizations;
using System.Text;

namespace SimpleTaskManager.WebApi.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddConfigurationServices(this IServiceCollection services, IConfiguration configuration)
		{
			var jwtConfig = configuration.GetSection("Jwt").Get<JwtTokensConfiguration>();
			if(jwtConfig != null)
			{
				services.AddSingleton(jwtConfig);
			}
			var passwordConfig = configuration.GetSection("Password").Get<PasswordConfiguration>();
			if(passwordConfig != null)
			{
				services.AddSingleton(passwordConfig);
			}
		}
		public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
		{
			var connString = configuration.GetConnectionString("DatabaseConnection");
			services.AddDbContext<SimpleTaskManagerDbContext>(options =>
			{
				options.UseSqlServer(connString, opt =>
				{
					opt.MigrationsAssembly(typeof(SimpleTaskManagerDbContext).Assembly.GetName().Name);
				});
			});
		}

		public static void AddRepositoryServices(this IServiceCollection services)
		{
			services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
		}

		public static void AddTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = configuration["Jwt:Issuer"],
					ValidAudience = configuration["Jwt:Audience"],
					ClockSkew = TimeSpan.Zero,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				};

				options.RequireHttpsMetadata = false;
			});

			services.AddScoped<IJwtTokensService, JwtTokensService>();
			services.AddScoped<IUserService, UserService>();
		}
		public static void AddAdditionalServices(this IServiceCollection services)
		{
			services.AddTransient<ICookiesService, CookiesService>();
			services.AddScoped<ITaskService, TaskService>();
		}
		public static void AddSwagger(this IServiceCollection services)
		{
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen(opt =>
			{
				opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Task Manager API", Version = "v1" });
				opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
				{
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = JwtBearerDefaults.AuthenticationScheme
				});

				opt.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = JwtBearerDefaults.AuthenticationScheme,
							},
								Scheme = JwtBearerDefaults.AuthenticationScheme,
								In = ParameterLocation.Header
						},
						new List<string>()
					}
				});
			});
		}
	}
}
