using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.DataAccess;

public interface IPostRepository
{
	Task<long> CreatePost(long userId, string text, CancellationToken cancellationToken);
	Task UpdatePost(long userId, long postId, string text, CancellationToken cancellationToken);
	Task DeletePost(long userId, long postId, CancellationToken cancellationToken);
	Task<Post> GetPostById(long postId, CancellationToken cancellationToken);
	Task<Post[]> GetFeed(long userId, int offset, int limit, CancellationToken cancellationToken);
}