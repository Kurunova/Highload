using StackExchange.Redis;

namespace SocialNetwork.Application.Services;

public class RedisCacheService
{
	private readonly IDatabase _cache;

	public RedisCacheService(IConnectionMultiplexer redis)
	{
		_cache = redis.GetDatabase();
	}

	public async Task AddToFeedAsync(long userId, string postJson, long createdAt)
	{
		var key = $"feed:{userId}";
		await _cache.SortedSetAddAsync(key, postJson, createdAt);
		await _cache.SortedSetRemoveRangeByRankAsync(key, 0, -1001); // Удалить все, кроме последних 1000
	}

	public async Task<string[]> GetFeedAsync(long userId, int count = 1000)
	{
		var key = $"feed:{userId}";
		return (await _cache.SortedSetRangeByScoreAsync(key, order: Order.Descending, take: count))
			.Select(p => p.ToString()).ToArray();
	}
}