using FluentMigrator;
using FluentMigrator.Runner;
using SocialNetwork.Counters.Grpc.Core.DataAccess;
using SocialNetwork.Counters.Grpc.Domain;

namespace SocialNetwork.Counters.Grpc.DataAccess;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMigration(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;
		var schema = configuration.GetSection("DatabaseSettings:Schema").Value ?? "schema_counters";
		
		serviceCollection
			.AddFluentMigratorCore()
			.AddLogging(lb => lb.AddFluentMigratorConsole())
			.ConfigureRunner(builder => builder
				.AddPostgres()
				.WithGlobalConnectionString(connectionString)
				.WithMigrationsIn(typeof(CustomMigration).Assembly)
				.ScanIn(typeof(CustomVersionTableMetaData).Assembly).For.VersionTableMetaData()
			);

        // using var scope = serviceCollection.BuildServiceProvider().CreateScope();
        // var processor = scope.ServiceProvider.GetRequiredService<IMigrationProcessor>();
        // processor.Execute($"SET search_path TO {schema};");
        
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