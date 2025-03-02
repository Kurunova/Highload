using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SocialNetwork.DataAccess.Extensions;

public static class ProgramExtension
{
	public static async Task RunOrMigrate(
		this IHost host,
		string options)
	{
		using var scope = host.Services.CreateScope();
		var logger = scope.ServiceProvider.GetRequiredService<ILogger<IMigrationRunner>>();
		
		logger.LogInformation($"Get input args: {options}");

		if (!string.IsNullOrEmpty(options) && options.Equals("migrateUp", StringComparison.InvariantCultureIgnoreCase))
		{
			using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
			
			var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
			runner.MigrateUp();
			
			logger.LogInformation($"DB migratred up");
		}
		else if (!string.IsNullOrEmpty(options) && options.StartsWith("migrateDown", StringComparison.InvariantCultureIgnoreCase))
		{
			using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
			
			var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
			var optionsArr = options.Split(" ");
			if (optionsArr.Length > 1)
			{
				var version = int.Parse(optionsArr[1]);
				runner.MigrateDown(version);
				logger.LogInformation($"DB migratred down to version {version}");
			}
			else
			{
				runner.MigrateDown(0);
				logger.LogInformation($"DB migratred down");
			}
		}
		else
		{
			await host.RunAsync();
		}
	}
}