namespace SocialNetwork.Application.Configurations;

public class RabbitMqSettings
{
	public string HostName { get; set; }
	public string UserName { get; set; }
	public string Password { get; set; }
	public string Exchange { get; set; }
	public string Queue { get; set; }
	public string RoutingKeyPrefix { get; set; }
}