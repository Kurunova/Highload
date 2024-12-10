using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Dialog.DataAccess.Configurations;
using SocialNetwork.Domain.DataAccess;

namespace SocialNetwork.Dialog.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDialogDatabase(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		serviceCollection.Configure<DatabaseSettings>(configuration.GetSection("DialogDbSettings"));
		serviceCollection.AddSingleton<IPostgresConnectionFactory, PostgresConnectionFactory>();
		serviceCollection.AddTransient<IDialogRepository, DialogRepository>();
		
		return serviceCollection;
	}
}