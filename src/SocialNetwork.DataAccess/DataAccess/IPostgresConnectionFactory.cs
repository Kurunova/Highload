using Npgsql;

namespace SocialNetwork.Domain.DataAccess;

public interface IPostgresConnectionFactory
{
	NpgsqlConnection CreateMasterConnection();
	NpgsqlConnection CreateReplicaConnection();
}