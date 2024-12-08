using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Application.Services;
using SocialNetwork.Domain.Services;

namespace SocialNetwork.Application.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		serviceCollection.AddTransient<IUserService, UserService>();
		serviceCollection.AddTransient<IPostService, PostService>();
		return serviceCollection;
	}
}