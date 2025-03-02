using SocialNetwork.Counters.Grpc.Domain;

namespace SocialNetwork.Counters.Grpc.DataAccess;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCountersDatabase(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		serviceCollection.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
		serviceCollection.AddSingleton<ICountersRepository, CountersRepository>();
		
		return serviceCollection;
	}
}