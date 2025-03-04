using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using SocialNetwork.Counters.Grpc;
using SocialNetwork.Counters.Grpc.Configurations;
using SocialNetwork.DataAccess.Extensions;

await Host
	.CreateDefaultBuilder(args)
	.ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>()
		.ConfigureKestrel(option => 
		{
			// if(int.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariableConstants.HTTP_PORT), out var httpPort))
			// 	option.Listen(IPAddress.Any, httpPort, options => options.Protocols = HttpProtocols.Http1);		
			if(int.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariableConstants.GRPC_PORT), out var grpcPort))
				option.Listen(IPAddress.Any, grpcPort, options => options.Protocols = HttpProtocols.Http2);	
		}))
	.UseSerilog()
	.Build()
	.RunOrMigrate(Environment.GetEnvironmentVariable(EnvironmentVariableConstants.RUN_OPTIONS));