namespace SocialNetwork.DataAccess.Configurations;

public class DatabaseSettings
{
	public string MasterConnectionString { get; set; }
	public List<string> ReplicaConnectionStrings { get; set; }
}