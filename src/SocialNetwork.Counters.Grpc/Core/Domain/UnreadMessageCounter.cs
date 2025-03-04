namespace SocialNetwork.Counters.Grpc.Domain;

public class UnreadMessageCounter
{
	public long UserId { get; set; }
	public int UnreadCount { get; set; }
}