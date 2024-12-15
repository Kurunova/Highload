using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialNetwork.Application.Configurations;
using SocialNetwork.Domain.Entities;
using StackExchange.Redis;

namespace SocialNetwork.Application.Services;

public class PostCacheService
{
	private readonly IDatabase _cache;
	private readonly RedisSettings _redisSettings;

	public PostCacheService(IConnectionMultiplexer redis, IOptions<RedisSettings> redisSettings)
	{
		_cache = redis.GetDatabase();
		_redisSettings = redisSettings.Value;
	}

	public async Task<List<Post>> GetFeedFromCache(long userId)
	{
		if (!_redisSettings.Enable)
			return null;
		
		var key = $"feed:{userId}";
		if (!await _cache.KeyExistsAsync(key))
			return null;
		
		var feedJson = (await _cache.SortedSetRangeByScoreAsync(key, order: Order.Descending, take: _redisSettings.FeedCount))
			.Select(p => p.ToString()).ToArray();
		if (feedJson.Length <= 0)
			return null;

		var posts = new List<Post>();
		foreach (var json in feedJson)
		{
			var post = JsonConvert.DeserializeObject<Post>(json);
			if (post != null)
				posts.Add(post);
		}
		
		return posts;
	}
	
	
	public async Task CreateFeedInCache(long userId, Post post)
	{
		var key = $"feed:{userId}";
		var postJson = JsonConvert.SerializeObject(post);
		
		await _cache.SortedSetAddAsync(key, postJson, post.CreatedAt.Ticks);
		await _cache.SortedSetRemoveRangeByRankAsync(key, 0, -( _redisSettings.FeedCount + 1)); // Удалить все, кроме последних (count) 1000
		var ttl = _redisSettings.TimeToLive;
		await _cache.KeyExpireAsync(key, ttl);
	}
	
	public async Task AppendPostToFeedInCache(long userId, Post post, long createdAt)
	{
		if(!_redisSettings.Enable)
			return;
		
		var postJson = JsonConvert.SerializeObject(post);
		
		var key = $"feed:{userId}";
		var keyExists = await _cache.KeyExistsAsync(key);
		if (!keyExists)
			return;

		await _cache.SortedSetAddAsync(key, postJson, createdAt);
		await _cache.SortedSetRemoveRangeByRankAsync(key, 0, -(_redisSettings.FeedCount + 1)); // Удалить все, кроме последних (count) 1000
	}
	
	public async Task UpdateFeedInCache(long userId, Post post)
	{
		if(!_redisSettings.Enable)
			return;
		
		await RemovePostFromFeedInCache(userId, post.Id);
		
		var key = $"feed:{userId}";
		var postJson = JsonConvert.SerializeObject(post);
		await _cache.SortedSetAddAsync(key, postJson, post.CreatedAt.Ticks);
	}

	public async Task RemovePostFromFeedInCache(long userId, long postId)
	{
		if(!_redisSettings.Enable)
			return;
		
		var key = $"feed:{userId}";
		
		var existingPosts = await _cache.SortedSetRangeByScoreAsync(key);

		foreach (var entry in existingPosts)
		{
			var post = JsonConvert.DeserializeObject<Post>(entry);
			if (post?.Id == postId)
			{
				await _cache.SortedSetRemoveAsync(key, entry);
				break;
			}
		}
	}
}