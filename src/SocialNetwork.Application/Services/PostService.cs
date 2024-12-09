﻿using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
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
			await AppendPostToSubscribers(post, cancellationToken);

		return post.Id;
	}

	public async Task UpdatePost(long userId, long postId, string text, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(text))
			throw new ValidationException("Post text cannot be empty.");

		var post = await _postRepository.UpdatePost(userId, postId, text, cancellationToken);;
		
		if (_redisSettings.Enable)
			await _redisCacheService.UpdateFeedInCache(userId, post);
	}

	public async Task DeletePost(long userId, long postId, CancellationToken cancellationToken)
	{
		await _postRepository.DeletePost(userId, postId, cancellationToken);
		
		if (_redisSettings.Enable)
			await _redisCacheService.RemovePostFromFeedInCache(userId, postId);
	}

	public async Task<Post> GetPostById(long postId, CancellationToken cancellationToken)
	{
		return await _postRepository.GetPostById(postId, cancellationToken);
	}

	public async Task<List<Post>> GetFeed(long userId, int offset, int limit, CancellationToken cancellationToken)
	{
		if(!_redisSettings.Enable)
			return await _postRepository.GetFeed(userId, offset, limit, cancellationToken);
		
		var posts = await _redisCacheService.GetFeedFromCache(userId);
		if (posts != null)
			return posts;
		
		posts = await _postRepository.GetFeed(userId, 0, _redisSettings.FeedCount, cancellationToken);
		foreach (var post in posts)
		{
			await _redisCacheService.CreateFeedInCache(userId, post);
		}
	
		return posts;
	}

	private async Task AppendPostToSubscribers(Post post, CancellationToken cancellationToken)
	{
		var subscriberIds = await _userRepository.GetSubscriberIds(post.AuthorUserId, cancellationToken);
		foreach (var subscriberId in subscriberIds)
		{
			await _redisCacheService.AppendPostToFeedInCache(subscriberId, post, post.CreatedAt.Ticks);
		}
	}
}