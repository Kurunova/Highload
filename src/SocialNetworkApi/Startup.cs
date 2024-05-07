using Serilog;
using SocialNetwork.Application.Extensions;
using SocialNetwork.DataAccess.Extensions;
using SocialNetworkApi.Extensions;

namespace SocialNetworkApi;

public sealed class Startup
{
	private readonly IConfiguration _configuration;

	public Startup(IConfiguration configuration)
	{
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			.CreateLogger();

		_configuration = configuration;
	}

	public void ConfigureServices(IServiceCollection serviceCollection)
	{
		serviceCollection.AddDatabase(_configuration);
		serviceCollection.AddApplication(_configuration);
		serviceCollection.AddJwtAuthorization(_configuration);

		serviceCollection.AddControllers();
		serviceCollection.AddEndpointsApiExplorer();
		serviceCollection.AddSwaggerGen();
	}

	public void Configure(IApplicationBuilder applicationBuilder)
	{
		applicationBuilder.UseRouting();
		applicationBuilder.UseSwagger();
		applicationBuilder.UseSwaggerUI();
		applicationBuilder.UseHttpsRedirection();

		applicationBuilder.UseEndpoints(endpointRouteBuilder =>
		{
			endpointRouteBuilder.MapControllers();
		});
	}
}