using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Application.Configurations;
using SocialNetwork.Application.Services;
using SocialNetwork.Domain.Services;
using StackExchange.Redis;

namespace SocialNetwork.Application.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		serviceCollection.AddTransient<IUserService, UserService>();
		serviceCollection.AddTransient<IPostService, PostService>();
		serviceCollection.AddTransient<IDialogService, DialogService>();
		serviceCollection.AddRedis(configuration);
		serviceCollection.AddRabbitMq(configuration);
		return serviceCollection;
	}

	private static IServiceCollection AddRedis(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		var redisSettingsSection = configuration.GetSection("Cache");
		if (redisSettingsSection == null)
			throw new InvalidOperationException("Cache settings section is missing in the configuration.");
		
		var redisSettings = redisSettingsSection.Get<RedisSettings>();
		if (redisSettings == null)
			throw new InvalidOperationException("Cache settings cannot be loaded from the section.");
		
		serviceCollection.Configure<RedisSettings>(redisSettingsSection);
		
		serviceCollection.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisSettings.RedisConnection));
		serviceCollection.AddTransient<PostCacheService>();
		return serviceCollection;
	}
	
	private static IServiceCollection AddRabbitMq(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		var rabbitMqSettingsSection = configuration.GetSection("RabbitMQ");
		if (rabbitMqSettingsSection == null)
			throw new InvalidOperationException("RabbitMQ settings section is missing in the configuration.");
		
		var rabbitMqSettings = rabbitMqSettingsSection.Get<RabbitMqSettings>();
		if (rabbitMqSettings == null)
			throw new InvalidOperationException("RabbitMQ settings cannot be loaded from the section.");
		
		serviceCollection.Configure<RabbitMqSettings>(rabbitMqSettingsSection);
		
		serviceCollection.AddTransient<RabbitMqService>();
		return serviceCollection;
	}
}