using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SocialNetwork.DataAccess.Extensions;
using SocialNetwork.DbMigrator.Configurations;

class Program
{
	static async Task Main(string[] args)
	{
		var host = Host.CreateDefaultBuilder(args)
			.ConfigureAppConfiguration((context, config) =>
			{
				config.AddEnvironmentVariables();
			})
			.ConfigureServices((context, services) =>
			{
				var configuration = context.Configuration;
				
				Log.Logger = new LoggerConfiguration()
					.ReadFrom.Configuration(configuration)
					.CreateLogger();
				
				services.AddDatabase(configuration);
			})
			.Build();
		
		using var scope = host.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<IMigrationRunner>>();

        var options = Environment.GetEnvironmentVariable(EnvironmentVariableConstants.RUN_OPTIONS);

        logger.LogInformation($"Get input args: {options}");

        if (!string.IsNullOrEmpty(options) && options.Equals("migrateUp", StringComparison.InvariantCultureIgnoreCase))
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));

            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();

            logger.LogInformation("DB migrated up");
        }
        else if (!string.IsNullOrEmpty(options) && options.StartsWith("migrateDown", StringComparison.InvariantCultureIgnoreCase))
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));

            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            var optionsArr = options.Split(" ");
            
            if (optionsArr.Length == 1)
            {
	            runner.Rollback(1);
	            logger.LogInformation($"DB migrated down to one step");
            }
            else if (optionsArr[1].Equals("All", StringComparison.InvariantCultureIgnoreCase))
            {
                runner.MigrateDown(0);
                logger.LogInformation("DB migrated down to version 0 (all migrations rolled back).");
            }
            else if (int.TryParse(optionsArr[1], out var targetVersion))
            {
	            runner.MigrateDown(targetVersion);
	            logger.LogInformation($"DB migrated down to version {targetVersion}");
            }
        }
        else
        {
	        logger.LogError($"Invalid settings");
        }
	}
}