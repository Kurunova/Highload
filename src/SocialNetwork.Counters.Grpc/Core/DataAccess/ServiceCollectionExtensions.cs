using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using SocialNetwork.Counters.Grpc.Domain;

namespace SocialNetwork.Counters.Grpc.DataAccess;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMigration(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;
		serviceCollection.AddFluentMigratorCore()
			.ConfigureRunner(builder => builder
				.AddPostgres().ScanIn(typeof(ServiceCollectionExtensions).Assembly)
				.For.Migrations())
			.AddOptions<ProcessorOptions>()
			.Configure(options =>
			{
				options.ProviderSwitches = "Force Quote=false";
				options.Timeout = TimeSpan.FromMinutes(10);
				options.ConnectionString = connectionString;
			});

		return serviceCollection;
	}
	
	public static IServiceCollection AddCountersDatabase(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		serviceCollection.AddMigration(configuration);
		serviceCollection.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
		serviceCollection.AddSingleton<ICountersRepository, CountersRepository>();
		
		return serviceCollection;
	}
}