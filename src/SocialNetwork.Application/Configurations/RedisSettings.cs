namespace SocialNetwork.Application.Configurations;

public class RedisSettings
{
	public bool Enable { get; set; }
	public string RedisConnection { get; set; }
	public TimeSpan TimeToLive { get; set; } = TimeSpan.FromMinutes(30);
	public int FeedCount { get; set; } = 1000;
}