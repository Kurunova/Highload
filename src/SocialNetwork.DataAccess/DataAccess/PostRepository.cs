using Microsoft.Extensions.Logging;
using SocialNetwork.Domain.DataAccess;
using SocialNetwork.Domain.Entities;

namespace SocialNetwork.DataAccess.DataAccess;

public class PostRepository : BaseRepository, IPostRepository
{
	public PostRepository(ILoggerFactory loggerFactory, IPostgresConnectionFactory connectionFactory) 
		: base(loggerFactory, connectionFactory)
	{
	}
	
	public async Task<long> CreatePost(long userId, string text, CancellationToken cancellationToken)
	{
		var sql = @"
        INSERT INTO Posts (AuthorUserId, Text)
        VALUES (@userId, @text)
        RETURNING Id";

		return await ExecuteAsync<long>(sql, new { userId, text }, cancellationToken);
	}

	public async Task UpdatePost(long userId, long postId, string text, CancellationToken cancellationToken)
	{
		var sql = @"
        UPDATE Posts
        SET Text = @text
        WHERE Id = @postId AND AuthorUserId = @userId";

		await ExecuteAsync(sql, new { postId, userId, text }, cancellationToken);
	}

	public async Task DeletePost(long userId, long postId, CancellationToken cancellationToken)
	{
		var sql = @"
        DELETE FROM Posts
        WHERE Id = @postId AND AuthorUserId = @userId";

		await ExecuteAsync(sql, new { postId, userId }, cancellationToken);
	}

	public async Task<Post> GetPostById(long postId, CancellationToken cancellationToken)
	{
		var sql = "SELECT * FROM Posts WHERE Id = @postId";

		return await QuerySingleOrDefaultAsync<Post>(sql, new { postId }, cancellationToken);
	}

	public async Task<Post[]> GetFeed(long userId, int offset, int limit, CancellationToken cancellationToken)
	{
		var sql = @"
        SELECT p.*
        FROM Posts p
        JOIN UserFriends uf ON uf.FriendId = p.AuthorUserId
        WHERE uf.UserId = @userId
        ORDER BY p.CreatedAt DESC
        OFFSET @offset LIMIT @limit";

		return (await QueryAsync<Post>(sql, new { userId, offset, limit }, cancellationToken)).ToArray();
	}
}