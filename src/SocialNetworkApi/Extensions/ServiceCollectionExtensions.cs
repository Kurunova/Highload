using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SocialNetworkApi.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddJwtAuthorization(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
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
					ValidIssuer = configuration.GetSection("Jwt:Issuer").Get<string>(),
					ValidAudience = configuration.GetSection("Jwt:Issuer").Get<string>(),
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt:Key").Get<string>()))
				};
			});
		return serviceCollection;
	}
}