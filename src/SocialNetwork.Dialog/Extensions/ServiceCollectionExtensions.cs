using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Dialog.Services;

namespace SocialNetwork.Dialog.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDialog(this IServiceCollection serviceCollection, IConfiguration configuration)
	{
		serviceCollection.AddTransient<IDialogService, DialogService>();
		return serviceCollection;
	}
}