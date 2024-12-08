using Microsoft.Extensions.Options;
using SocialNetwork.Application.Configurations;
using StackExchange.Redis;

namespace SocialNetwork.Application.Services;

public class RedisCacheService
{
	private readonly IDatabase _cache;
	private readonly RedisSettings _redisSettings;

	public RedisCacheService(IConnectionMultiplexer redis, IOptions<RedisSettings> redisSettings)
	{
		_cache = redis.GetDatabase();
		_redisSettings = redisSettings.Value;
	}

	public async Task AddToFeedAsync(long userId, string postJson, long createdAt)
	{
		var key = $"feed:{userId}";
		var keyExists = await _cache.KeyExistsAsync(key);
		if (!keyExists)
			return;

		await _cache.SortedSetAddAsync(key, postJson, createdAt);
		await _cache.SortedSetRemoveRangeByRankAsync(key, 0, -1001); // Удалить все, кроме последних 1000
	}
	
	public async Task AddFeedAsync(long userId, string postJson, long createdAt)
	{
		var key = $"feed:{userId}";
		await _cache.SortedSetAddAsync(key, postJson, createdAt);
		await _cache.SortedSetRemoveRangeByRankAsync(key, 0, -1001); // Удалить все, кроме последних 1000
		var ttl = _redisSettings.TimeToLive;
		await _cache.KeyExpireAsync(key, ttl);
	}
	
	public async Task<string[]> GetFeedAsync(long userId, int count = 1000)
	{
		var key = $"feed:{userId}";
		if (!await _cache.KeyExistsAsync(key))
		 	return Array.Empty<string>();
		
		return (await _cache.SortedSetRangeByScoreAsync(key, order: Order.Descending, take: count))
			.Select(p => p.ToString()).ToArray();
	}
}