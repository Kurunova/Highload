namespace SocialNetwork.Dialog.DataAccess.Configurations;

public class DatabaseSettings
{
	public bool UseTarantoolDb { get; set; }
	public PostgreSqlDbSettings PostgreSqlDbSettings { get; set; }
	public TarantoolDbSettings TarantoolDbSettings { get; set; }
}