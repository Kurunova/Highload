using Npgsql;

namespace SocialNetwork.Domain.DataAccess;

public interface IPostgresConnectionFactory
{
	NpgsqlConnection CreateConnection();
}