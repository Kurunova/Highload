﻿using Microsoft.OpenApi.Models;
using Serilog;
using SocialNetwork.Application.Extensions;
using SocialNetwork.DataAccess.Extensions;
using SocialNetwork.Dialog.DataAccess.Extensions;
using SocialNetworkApi.Extensions;
using SocialNetworkApi.Hubs;
using SocialNetworkApi.Middlewares;

namespace SocialNetworkApi;

public sealed class Startup
{
	private readonly IConfiguration _configuration;

	public Startup(IConfiguration configuration)
	{
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			.WriteTo.Console()
			.CreateLogger();

		_configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection serviceCollection)
	{
		serviceCollection.AddDatabase(_configuration);
		serviceCollection.AddDialogDatabase(_configuration);
		serviceCollection.AddApplication(_configuration);
		serviceCollection.AddJwt(_configuration);
		serviceCollection.AddWebSockets(_configuration);
		
		serviceCollection.AddControllers();
		serviceCollection.AddEndpointsApiExplorer();
		serviceCollection.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer"
			});
			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						},
						Scheme = "oauth2",
						Name = "Bearer",
						In = ParameterLocation.Header
					},
					Array.Empty<string>()
				}
			});
		});
	}

	public void Configure(IApplicationBuilder applicationBuilder)
	{
		applicationBuilder.UseMiddleware<ExceptionMiddleware>();
		applicationBuilder.UseWebSockets();
		applicationBuilder.UseRouting();
		applicationBuilder.UseHttpsRedirection();
		
		applicationBuilder.UseAuthentication();
		applicationBuilder.UseAuthorization();
		
		applicationBuilder.UseSwagger();
		applicationBuilder.UseSwaggerUI();
		
		applicationBuilder.UseEndpoints(endpointRouteBuilder =>
		{
			endpointRouteBuilder.MapControllers();
			endpointRouteBuilder.MapHub<PostFeedHub>("/post/feed/posted"); // Маршрут для WebSocket
		});
	}
}