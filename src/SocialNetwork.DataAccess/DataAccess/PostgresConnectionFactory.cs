using Microsoft.Extensions.Options;
using Npgsql;
using SocialNetwork.DataAccess.Configurations;
using SocialNetwork.Domain.DataAccess;

namespace SocialNetwork.DataAccess.DataAccess;

public class PostgresConnectionFactory : IPostgresConnectionFactory
{
	private readonly string _connectionString;

	public PostgresConnectionFactory(IOptions<DatabaseSettings> databaseSettings)
	{
		_connectionString = databaseSettings?.Value?.ConnectionString ?? throw new ArgumentException("Connection string is mandatory");
	}

	public NpgsqlConnection CreateConnection()
	{
		return new NpgsqlConnection(_connectionString);
	}
}