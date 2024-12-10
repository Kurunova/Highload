using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SocialNetworkApi.Configurations;
using SocialNetworkApi.Services;

namespace SocialNetworkApi.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddJwt(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		serviceCollection.AddTransient<JwtTokenService>();
		serviceCollection.AddJwtAuthorization(configuration);
		return serviceCollection;
	}

	public static IServiceCollection AddJwtAuthorization(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		var jwtSettingsSection = configuration.GetSection("Jwt");
		if (jwtSettingsSection == null)
			throw new InvalidOperationException("JWT settings section is missing in the configuration.");
		
		var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
		if (jwtSettings == null)
			throw new InvalidOperationException("JWT settings cannot be loaded from the section.");
		
		serviceCollection.Configure<JwtSettings>(jwtSettingsSection);
		
		serviceCollection.AddAuthentication(options =>
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
					ValidIssuer = jwtSettings.Issuer,
					ValidAudience = jwtSettings.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
				};
				
				// Добавление обработчика для извлечения токена из строки запроса
				options.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						// Попытка получить токен из строки запроса
						var accessToken = context.Request.Query["access_token"];
                    
						// Если токен передан, устанавливаем его в контекст
						if (!string.IsNullOrEmpty(accessToken))
						{
							context.Token = accessToken;
						}

						return Task.CompletedTask;
					}
				};
			});
		return serviceCollection;
	}
}