using Microsoft.Extensions.Logging;
using SocialNetwork.Dialog.Entities;

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
            INSERT INTO dialog_messages (dialog_id, from_user_id, to_user_id, text, sent_at)
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

	public async Task<DialogMessage?> GetMessageByIdAsync(long messageId, CancellationToken cancellationToken)
	{
		var sql = @"
            SELECT message_id as MessageId, 
                from_user_id AS From, 
                to_user_id AS To, 
                text, 
                sent_at AS SentAt, 
                is_read AS IsRead
            FROM dialog_messages
            WHERE id = @MessageId
            ORDER BY sent_at";

		var messages = await QueryAsync<DialogMessage>(sql, new { MessageId = messageId }, cancellationToken);

		return messages.FirstOrDefault();
	}
	
	public async Task<IEnumerable<DialogMessage>> GetMessagesBetweenUsersAsync(long userId1, long userId2, CancellationToken cancellationToken)
	{
		var dialogId = userId1 < userId2
			? $"{userId1}_{userId2}"
			: $"{userId2}_{userId1}";
		
		var sql = @"
            SELECT message_id as MessageId, 
                from_user_id AS From, 
                to_user_id AS To, 
                text, 
                sent_at AS SentAt, 
                is_read AS IsRead
            FROM dialog_messages
            WHERE dialog_id = @DialogId
            ORDER BY sent_at";

		return await QueryAsync<DialogMessage>(sql, new { DialogId = dialogId }, cancellationToken);
	}
	
	public async Task<bool> UpdateMessageReadStatusAsync(long messageId, bool isRead, CancellationToken cancellationToken)
	{
		const string sql = "UPDATE dialog_messages SET is_read = @IsRead WHERE id = @MessageId";
		var parameters = new { MessageId = messageId, IsRead = isRead };

		return await ExecuteAsync(sql, parameters, cancellationToken);
	}
}