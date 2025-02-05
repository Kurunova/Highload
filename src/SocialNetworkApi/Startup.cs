using Microsoft.OpenApi.Models;
using Serilog;
using SocialNetwork.Application.Extensions;
using SocialNetwork.DataAccess.Extensions;
// using SocialNetwork.Dialog.DataAccess.Extensions;
// using SocialNetwork.Dialog.Extensions;
using SocialNetworkApi.BackgroundServices;
using SocialNetworkApi.Extensions;
using SocialNetworkApi.Hubs;
using SocialNetworkApi.Interceptors;
//using SocialNetworkApi.Logs;//
using SocialNetworkApi.Middlewares;

namespace SocialNetworkApi;

public sealed class Startup
{
	private readonly IConfiguration _configuration;

	public Startup(IConfiguration configuration)
	{
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			.Enrich.FromLogContext()
			//.WriteTo.Console()
			.CreateLogger();

		_configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection serviceCollection)
	{
		serviceCollection.AddDatabase(_configuration);
		serviceCollection.AddApplication(_configuration);
		serviceCollection.AddJwt(_configuration);
		serviceCollection.AddWebSockets(_configuration);
		
		serviceCollection.AddHostedService<PostFeedConsumer>();
		
		// Grpc
		serviceCollection.AddTransient<RequestIdInterceptor>();
		var dialogServiceAddress = _configuration.GetValue<string>("DialogService:GrpcConnectionString");
		serviceCollection.AddGrpcClient<SocialNetwork.Dialog.Grpc.V1.GrpcDialogService.GrpcDialogServiceClient>(
			"GrpcDialogServiceClientV1", 
			options =>
			{
				options.Address = new Uri(dialogServiceAddress);
			})
			.AddInterceptor<RequestIdInterceptor>();
		serviceCollection.AddGrpcClient<SocialNetwork.Dialog.Grpc.V2.GrpcDialogService.GrpcDialogServiceClient>(
			"GrpcDialogServiceClientV2", 
			options =>
			{
				options.Address = new Uri(dialogServiceAddress);
			})
			.AddInterceptor<RequestIdInterceptor>();
		
		serviceCollection.AddHttpContextAccessor();
		
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
		applicationBuilder.UseMiddleware<RequestIdMiddleware>();
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