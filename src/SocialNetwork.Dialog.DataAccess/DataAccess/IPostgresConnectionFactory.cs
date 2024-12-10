using Npgsql;

namespace SocialNetwork.Dialog.DataAccess;


public interface IPostgresConnectionFactory
{
	NpgsqlConnection CreateMasterConnection();
	//NpgsqlConnection CreateReplicaConnection();
}