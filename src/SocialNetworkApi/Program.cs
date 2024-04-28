using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using SocialNetwork.DataAccess.Extensions;
using SocialNetworkApi;
using SocialNetworkApi.Configurations;

await Host
	.CreateDefaultBuilder(args)
	.ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>()
		.ConfigureKestrel(option => 
		{
			if(int.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariableConstants.HTTP_PORT), out var httpPort))
				option.Listen(IPAddress.Any, httpPort, options => options.Protocols = HttpProtocols.Http1);			
		}))
	.UseSerilog()
	.Build()
	.RunOrMigrate(Environment.GetEnvironmentVariable(EnvironmentVariableConstants.RUN_OPTIONS));