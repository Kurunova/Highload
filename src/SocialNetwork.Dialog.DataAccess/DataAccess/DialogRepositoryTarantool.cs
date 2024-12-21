using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProGaudi.Tarantool.Client;
using SocialNetwork.Dialog.DataAccess.Configurations;
using SocialNetwork.Domain.DataAccess;
using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Dialog.DataAccess;

/// <summary>
/// https://www.tarantool.io/en/doc/latest/connector/community/csharp/
/// </summary>
public class DialogRepositoryTarantool : IDialogRepository
{
	private readonly ILogger<DialogRepository> _logger;
	private readonly ISpace _dialogMessagesSpace;

	public DialogRepositoryTarantool(ILoggerFactory loggerFactory, IOptions<DatabaseSettings> databaseSettings)
	{
		_logger = loggerFactory.CreateLogger<DialogRepository>();
		
		_dialogMessagesSpace = Initialize(databaseSettings.Value?.TarantoolDbSettings?.ConnectionString, databaseSettings.Value?.TarantoolDbSettings?.SpaceName)
			.GetAwaiter().GetResult();
	}
	
	static async Task<ISpace> Initialize(string connectionString, string spaceName)
	{
		var box = await Box.Connect(connectionString);
		var schema = box.GetSchema();
		var space = await schema.GetSpace(spaceName);
		return space; 
	}
	
	public async Task AddMessageAsync(DialogMessage message, CancellationToken cancellationToken)
	{
		var dialogId = message.To < message.From
			? $"{message.To}_{message.From}"
			: $"{message.From}_{message.To}";

		try
		{
			await _dialogMessagesSpace.Insert(new object[]
			{
				dialogId,
				message.From,
				message.To,
				message.Text,
				message.SentAt.ToUniversalTime().ToString("O") // ISO 8601 для совместимости
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error while adding message to Tarantool");
			throw;
		}
	}

	public async Task<IEnumerable<DialogMessage>> GetMessagesBetweenUsersAsync(long userId1, long userId2, CancellationToken cancellationToken)
	{
		var dialogId = userId1 < userId2
			? $"{userId1}_{userId2}"
			: $"{userId2}_{userId1}";

		try
		{
			var result = await _dialogMessagesSpace.Select<string, DialogMessage>(dialogId);

			return result.Data.ToList();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error while retrieving messages from Tarantool");
			throw;
		}
	}
}