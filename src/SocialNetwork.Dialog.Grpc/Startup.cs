using Serilog;
using SocialNetwork.Dialog.DataAccess.Extensions;
using SocialNetwork.Dialog.Extensions;
using SocialNetwork.Dialog.Grpc.Services;

namespace SocialNetwork.Dialog.Grpc;

public sealed class Startup
{
	private readonly IConfiguration _configuration;

	public Startup(IConfiguration configuration)
	{
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			//.Enrich.WithMemoryUsage()
			.CreateLogger();

		_configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection serviceCollection)
	{
		serviceCollection.AddDialogDatabase(_configuration);
		serviceCollection.AddDialog(_configuration);
		
		serviceCollection.AddGrpc(options =>
		{
			//options.Interceptors.Add<LoggerInterceptor>();
		});
		serviceCollection.AddGrpcReflection();
	}

	public void Configure(IApplicationBuilder applicationBuilder)
	{
		applicationBuilder.UseRouting();
		applicationBuilder.UseHttpsRedirection();

		applicationBuilder.UseEndpoints(endpointRouteBuilder =>
		{
			endpointRouteBuilder.MapGrpcService<DialogServiceControllerV1>();
			endpointRouteBuilder.MapGrpcService<DialogServiceControllerV2>();
			endpointRouteBuilder.MapGrpcReflectionService();
			endpointRouteBuilder.MapGet("", () => "Hello Wold!");
		});
	}
}