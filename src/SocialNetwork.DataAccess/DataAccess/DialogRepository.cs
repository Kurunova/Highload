using Microsoft.Extensions.Logging;
using SocialNetwork.Domain.DataAccess;
using SocialNetwork.Domain.Entities;

namespace SocialNetwork.DataAccess.DataAccess;

public class DialogRepository : BaseRepository, IDialogRepository
{
	public DialogRepository(ILoggerFactory loggerFactory, IPostgresConnectionFactory connectionFactory)
		: base(loggerFactory, connectionFactory) { }

	public async Task AddMessageAsync(DialogMessage message, CancellationToken cancellationToken)
	{
		var sql = @"
            INSERT INTO dialog_messages (from_user_id, to_user_id, text, sent_at)
            VALUES (@From, @To, @Text, @SentAt)";
        
		await ExecuteAsync(sql, message, cancellationToken);
	}

	public async Task<IEnumerable<DialogMessage>> GetMessagesBetweenUsersAsync(long userId1, long userId2, CancellationToken cancellationToken)
	{
		var sql = @"
            SELECT from_user_id AS From, to_user_id AS To, text, sent_at AS SentAt
            FROM dialog_messages
            WHERE (from_user_id = @UserId1 AND to_user_id = @UserId2)
               OR (from_user_id = @UserId2 AND to_user_id = @UserId1)
            ORDER BY sent_at";

		return await QueryAsync<DialogMessage>(sql, new { UserId1 = userId1, UserId2 = userId2 }, cancellationToken);
	}
}