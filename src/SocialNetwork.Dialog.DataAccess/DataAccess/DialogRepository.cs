using Microsoft.Extensions.Logging;
using SocialNetwork.Domain.DataAccess;
using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Dialog.DataAccess;

public class DialogRepository : BaseRepository, IDialogRepository
{
	public DialogRepository(ILoggerFactory loggerFactory, IPostgresConnectionFactory connectionFactory)
		: base(loggerFactory, connectionFactory) { }

	public async Task AddMessageAsync(DialogMessage message, CancellationToken cancellationToken)
	{
		var dialogId = message.To < message.From
			? $"{message.To}_{message.From}"
			: $"{message.From}_{message.To}";
		var sql = @"
            INSERT INTO citus.dialog_messages (dialog_id, from_user_id, to_user_id, text, sent_at)
            VALUES (@DialogId, @From, @To, @Text, @SentAt)";
        
		await ExecuteAsync(sql, new
		{
			DialogId = dialogId, 
			From = message.From,
			To = message.To,
			Text = message.Text,
			SentAt = message.SentAt,
		}, cancellationToken);
	}

	public async Task<IEnumerable<DialogMessage>> GetMessagesBetweenUsersAsync(long userId1, long userId2, CancellationToken cancellationToken)
	{
		var dialogId = userId1 < userId2
			? $"{userId1}_{userId2}"
			: $"{userId2}_{userId1}";
		
		var sql = @"
            SELECT from_user_id AS From, to_user_id AS To, text, sent_at AS SentAt
            FROM citus.dialog_messages
            WHERE dialog_id = @DialogId
            ORDER BY sent_at";

		return await QueryAsync<DialogMessage>(sql, new { DialogId = dialogId }, cancellationToken);
	}
}