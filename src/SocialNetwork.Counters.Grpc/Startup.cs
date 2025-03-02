using Serilog;
using SocialNetwork.Counters.Grpc.DataAccess;
using SocialNetwork.Counters.Grpc.Services;

namespace SocialNetwork.Counters.Grpc;

public class Startup
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
		serviceCollection.AddCountersDatabase(_configuration);
		// serviceCollection.AddCounters(_configuration);
		
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
			endpointRouteBuilder.MapGrpcService<CountersServiceControllerV1>();
			endpointRouteBuilder.MapGrpcReflectionService();
			endpointRouteBuilder.MapGet("", () => "Hello Wold!");
		});
	}
}