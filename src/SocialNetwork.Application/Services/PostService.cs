using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialNetwork.Application.Configurations;
using SocialNetwork.Application.Models;
using SocialNetwork.Domain.DataAccess;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Services;

namespace SocialNetwork.Application.Services;

public class PostService : IPostService
{
	private readonly IPostRepository _postRepository;
	private readonly IUserRepository _userRepository;
	private readonly PostCacheService _postCacheService;
	private readonly RedisSettings _redisSettings;
	private readonly RabbitMqService _rabbitMqService;

	public PostService(
		IPostRepository postRepository, 
		IUserRepository userRepository, 
		PostCacheService postCacheService,
		IOptions<RedisSettings> redisSettings, 
		RabbitMqService rabbitMqService)
	{
		_postRepository = postRepository;
		_userRepository = userRepository;
		_postCacheService = postCacheService;
		_redisSettings = redisSettings.Value;
		_rabbitMqService = rabbitMqService;
	}
	
	public async Task<Post> CreatePost(long userId, string text, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(text))
			throw new ValidationException("Post text cannot be empty.");

		var post = await _postRepository.CreatePost(userId, text, cancellationToken);

		var postFeedMessage = new PostFeedMessage
		{
			Operation = PostFeedOperation.Created,
			Post = post
		};
		var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(postFeedMessage));
		await _rabbitMqService.PublishPostEvent(userId.ToString(), body, cancellationToken);
		
		return post;
	}

	public async Task UpdatePost(long userId, long postId, string text, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(text))
			throw new ValidationException("Post text cannot be empty.");

		var post = await _postRepository.UpdatePost(userId, postId, text, cancellationToken);;

		var postFeedMessage = new PostFeedMessage
		{
			Operation = PostFeedOperation.Updated,
			Post = post
		};
		var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(postFeedMessage));
		await _rabbitMqService.PublishPostEvent(userId.ToString(), body, cancellationToken);
	}

	public async Task DeletePost(long userId, long postId, CancellationToken cancellationToken)
	{
		await _postRepository.DeletePost(userId, postId, cancellationToken);
		
		var postFeedMessage = new PostFeedMessage
		{
			Operation = PostFeedOperation.Deleted,
			Post = new Post { Id = postId }
		};
		var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(postFeedMessage));
		await _rabbitMqService.PublishPostEvent(userId.ToString(), body, cancellationToken);
	}

	public async Task<Post> GetPostById(long postId, CancellationToken cancellationToken)
	{
		return await _postRepository.GetPostById(postId, cancellationToken);
	}

	public async Task<List<Post>> GetFeed(long userId, int offset, int limit, CancellationToken cancellationToken)
	{
		if(!_redisSettings.Enable)
			return await _postRepository.GetFeed(userId, offset, limit, cancellationToken);
		
		var posts = await _postCacheService.GetFeedFromCache(userId);
		if (posts != null)
			return posts;
		
		posts = await _postRepository.GetFeed(userId, 0, _redisSettings.FeedCount, cancellationToken);
		foreach (var post in posts)
		{
			await _postCacheService.CreateFeedInCache(userId, post);
		}
	
		return posts;
	}
}