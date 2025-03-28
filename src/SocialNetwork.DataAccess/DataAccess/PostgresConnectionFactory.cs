﻿using Microsoft.Extensions.Options;
using Npgsql;
using SocialNetwork.DataAccess.Configurations;
using SocialNetwork.Domain.DataAccess;

namespace SocialNetwork.DataAccess.DataAccess;

public class PostgresConnectionFactory : IPostgresConnectionFactory
{
	private readonly string _masterConnectionString;
	private readonly string _replicaConnectionString;
	//private readonly List<string> _replicaConnectionStrings;
	//private int _lastUsedReplicaIndex = 0;

	public PostgresConnectionFactory(IOptions<DatabaseSettings> databaseSettings)
	{
		_masterConnectionString = databaseSettings?.Value?.MasterConnectionString ?? throw new ArgumentException("Master connection string is mandatory");
		_replicaConnectionString = databaseSettings?.Value?.ReplicaConnectionString ?? throw new ArgumentException("Replica connection string is mandatory");
		//_replicaConnectionStrings = databaseSettings?.Value?.ReplicaConnectionStrings ?? throw new ArgumentException("Replica connection strings are mandatory");
	}

	public NpgsqlConnection CreateMasterConnection()
	{
		return new NpgsqlConnection(_masterConnectionString);
	}

	public NpgsqlConnection CreateReplicaConnection()
	{
		return new NpgsqlConnection(_replicaConnectionString);
	}
	
	// public NpgsqlConnection CreateReplicaConnection()
	// {
	// 	var replicaConnectionString = GetNextReplicaConnectionString();
	// 	return new NpgsqlConnection(replicaConnectionString);
	// }

	// private string GetNextReplicaConnectionString()
	// {
	// 	var replicaConnectionString = _replicaConnectionStrings[_lastUsedReplicaIndex];
	// 	_lastUsedReplicaIndex = (_lastUsedReplicaIndex + 1) % _replicaConnectionStrings.Count;
	// 	return replicaConnectionString;
	// }
}