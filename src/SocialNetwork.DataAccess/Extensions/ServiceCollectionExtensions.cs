using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.DataAccess.Configurations;
using SocialNetwork.DataAccess.DataAccess;
using SocialNetwork.Domain.DataAccess;

namespace SocialNetwork.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMigration(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		var connectionString = configuration.GetSection("DatabaseSettings:MasterConnectionString").Value;
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
	
	public static IServiceCollection AddDatabase(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		serviceCollection.AddMigration(configuration);
		serviceCollection.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
		serviceCollection.AddSingleton<IPostgresConnectionFactory, PostgresConnectionFactory>();
		serviceCollection.AddTransient<IUserRepository, UserRepository>();
		
		return serviceCollection;
	}
}