namespace SocialNetwork.Counters.Grpc.Domain;

public interface ICountersRepository
{
	Task<int> GetUnreadMessagesCountAsync(long userId, CancellationToken cancellationToken);
	Task IncrementUnreadMessagesCountAsync(long userId, int count, CancellationToken cancellationToken);
	Task DecrementUnreadMessagesCountAsync(long userId, int count, CancellationToken cancellationToken);
}