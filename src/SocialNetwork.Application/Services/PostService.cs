using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialNetwork.Application.Configurations;
using SocialNetwork.Domain.DataAccess;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Services;

namespace SocialNetwork.Application.Services;

public class PostService : IPostService
{
	private readonly IPostRepository _postRepository;
	private readonly IUserRepository _userRepository;
	private readonly RedisCacheService _redisCacheService;
	private readonly RedisSettings _redisSettings;

	public PostService(
		IPostRepository postRepository, 
		IUserRepository userRepository, 
		RedisCacheService redisCacheService,
		IOptions<RedisSettings> redisSettings)
	{
		_postRepository = postRepository;
		_userRepository = userRepository;
		_redisCacheService = redisCacheService;
		_redisSettings = redisSettings.Value;
	}
	
	public async Task<long> CreatePost(long userId, string text, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(text))
			throw new ValidationException("Post text cannot be empty.");

		var post = await _postRepository.CreatePost(userId, text, cancellationToken);
		
		if(_redisSettings.Enable)
			await UpdateFeedInCache(post, cancellationToken);

		return post.Id;
	}

	public async Task UpdatePost(long userId, long postId, string text, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(text))
			throw new ValidationException("Post text cannot be empty.");

		await _postRepository.UpdatePost(userId, postId, text, cancellationToken);
	}

	public async Task DeletePost(long userId, long postId, CancellationToken cancellationToken)
	{
		await _postRepository.DeletePost(userId, postId, cancellationToken);
	}

	public async Task<Post> GetPostById(long postId, CancellationToken cancellationToken)
	{
		return await _postRepository.GetPostById(postId, cancellationToken);
	}

	public async Task<List<Post>> GetFeed(long userId, int offset, int limit, CancellationToken cancellationToken)
	{
		if(!_redisSettings.Enable)
			return await _postRepository.GetFeed(userId, offset, limit, cancellationToken);
		
		var posts = await GetFeedFromCache(userId);
		if (posts != null)
			return posts;
		
		posts = await _postRepository.GetFeed(userId, 0, 1000, cancellationToken);
		await CreateFeedInCache(userId, posts);

		return posts;
	}

	private async Task<List<Post>> GetFeedFromCache(long userId)
	{
		var feedJson = await _redisCacheService.GetFeedAsync(userId);
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

	private async Task CreateFeedInCache(long userId, List<Post> posts)
	{
		foreach (var post in posts)
		{
			var postJson = JsonConvert.SerializeObject(post);
			await _redisCacheService.AddFeedAsync(userId, postJson, post.CreatedAt.Ticks);
		}
	}
	
	private async Task UpdateFeedInCache(Post post, CancellationToken cancellationToken)
	{
		var subscriberIds = await _userRepository.GetSubscriberIds(post.AuthorUserId, cancellationToken);
		foreach (var subscriberId in subscriberIds)
		{
			var postJson = JsonConvert.SerializeObject(post);
			await _redisCacheService.AddToFeedAsync(subscriberId, postJson, post.CreatedAt.Ticks);
		}
	}
}