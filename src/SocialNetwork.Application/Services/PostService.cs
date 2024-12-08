using System.ComponentModel.DataAnnotations;
using SocialNetwork.Domain.DataAccess;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Services;

namespace SocialNetwork.Application.Services;

public class PostService : IPostService
{
	private readonly IPostRepository _postRepository;

	public PostService(IPostRepository postRepository)
	{
		_postRepository = postRepository;
	}
	
	public async Task<long> CreatePost(long userId, string text, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(text))
			throw new ValidationException("Post text cannot be empty.");

		return await _postRepository.CreatePost(userId, text, cancellationToken);
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

	public async Task<Post[]> GetFeed(long userId, int offset, int limit, CancellationToken cancellationToken)
	{
		return await _postRepository.GetFeed(userId, offset, limit, cancellationToken);
	}
}