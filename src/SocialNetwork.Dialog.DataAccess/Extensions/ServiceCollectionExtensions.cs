﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Dialog.DataAccess.Configurations;

namespace SocialNetwork.Dialog.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDialogDatabase(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		var databaseSettingsSection = configuration.GetSection("DialogDbSettings");
		if (databaseSettingsSection == null)
			throw new InvalidOperationException("DialogDbSettings section is missing in the configuration.");
		
		var databaseSettings = databaseSettingsSection.Get<DatabaseSettings>();
		if (databaseSettings == null)
			throw new InvalidOperationException("DialogDbSettings cannot be loaded from the section.");
		
		serviceCollection.Configure<DatabaseSettings>(databaseSettingsSection);
		
		serviceCollection.AddSingleton<IPostgresConnectionFactory, PostgresConnectionFactory>();

		if (!databaseSettings.UseTarantoolDb)
		{
			serviceCollection.AddSingleton<IDialogRepository, DialogRepository>();
		}
		else
		{
			serviceCollection.AddSingleton<IDialogRepository, DialogRepositoryTarantool>();
		}
		
		return serviceCollection;
	}
}