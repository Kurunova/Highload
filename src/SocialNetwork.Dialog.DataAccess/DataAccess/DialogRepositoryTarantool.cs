﻿using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using SocialNetwork.Dialog.DataAccess.Configurations;
using SocialNetwork.Dialog.Entities;

namespace SocialNetwork.Dialog.DataAccess;

/// <summary>
/// https://www.tarantool.io/en/doc/latest/connector/community/csharp/
/// </summary>
public class DialogRepositoryTarantool : IDialogRepository
{
	private readonly ILogger<DialogRepository> _logger;
	private readonly ISpace _dialogMessagesSpace;
	private static Box Box;

	public DialogRepositoryTarantool(ILoggerFactory loggerFactory, IOptions<DatabaseSettings> databaseSettings)
	{
		_logger = loggerFactory.CreateLogger<DialogRepository>();

		if (databaseSettings.Value?.TarantoolDbSettings == null) throw new ArgumentException("TarantoolDbSettings is mandatory");
		try
		{
			var connectionString = $"{databaseSettings.Value.TarantoolDbSettings.Host}:{databaseSettings.Value.TarantoolDbSettings.Port}";
			// var context = new MsgPackContext();
			// var clientOptions = new ClientOptions(connectionString, context: context);
			// var box = new Box(clientOptions);
			//box.Connect().GetAwaiter().GetResult();
			//Box = box;
			
			Box = Box.Connect(connectionString).GetAwaiter().GetResult();

			_logger.LogInformation("Tarantool Db connection established");

			_dialogMessagesSpace = GetSpace(Box, databaseSettings.Value?.TarantoolDbSettings?.SpaceName)
				.GetAwaiter().GetResult();
			_logger.LogInformation("Tarantool Db space setup finished");
		}
		catch (Exception ex)
		{
			_logger.LogInformation($"Tarantool Db error: {ex}");
		}
	}
	
	private async Task<ISpace> GetSpace(Box box, string spaceName)
	{
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
			await Box.Call<TarantoolTuple<string, long, long, string, string>, TarantoolTuple<long, string, long, long, string, string, bool>>(
				"add_dialog_message",
				new TarantoolTuple<string, long, long, string, string>(
					dialogId, message.From,  message.To, message.Text, message.SentAt.ToUniversalTime().ToString("O") 
				));
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
			// var dataResponse = await _dialogMessagesSpace
			// 	.Select<TarantoolTuple<string>, TarantoolTuple<long, string, long, long, string, string>>(new TarantoolTuple<string>(dialogId));
			// var dialogMessagesTuple = dataResponse.Data;

			// var dataResponse = await Box.Call<TarantoolTuple<string>, TarantoolTuple<long, string, long, long, string, string>[]>(
			// 	"get_dialog_messages",
			// 	new TarantoolTuple<string>(dialogId)
			// );
			// var dialogMessagesTuple = dataResponse.Data[0];
			
			var dataResponse = await Box.Call<TarantoolTuple<string, int, int>, TarantoolTuple<long, string, long, long, string, string, bool>[]>(
				"get_dialog_messages_paginated",
				new TarantoolTuple<string, int, int>(dialogId, 100, 0)
			);
			var dialogMessagesTuple = dataResponse.Data[0];
			
			var result = dialogMessagesTuple.Select(p => new DialogMessage
			{
				MessageId = p.Item1,
				From = p.Item3,
				To = p.Item4,
				Text = p.Item5,
				SentAt = DateTime.TryParse(p.Item6, null, DateTimeStyles.RoundtripKind, out DateTime parsedDt)
					? parsedDt
					: DateTime.MinValue,
				IsRead = p.Item7
			});

			return result;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error while retrieving messages from Tarantool");
			throw;
		}
	}
	
	public async Task<DialogMessage?> GetMessageByIdAsync(long messageId, CancellationToken cancellationToken)
	{
		try
		{
			var dataResponse = await Box.Call<TarantoolTuple<long>, TarantoolTuple<long, string, long, long, string, string, bool>>(
				"get_message_by_id",
				new TarantoolTuple<long>(messageId)
			);

			var dialogMessagesTuple = dataResponse.Data[0];

			// var result = dialogMessagesTuple.Select(p => new DialogMessage
			// {
			// 	MessageId = p.Item1,
			// 	From = p.Item3,
			// 	To = p.Item4,
			// 	Text = p.Item5,
			// 	SentAt = DateTime.TryParse(p.Item6, null, DateTimeStyles.RoundtripKind, out DateTime parsedDt)
			// 		? parsedDt
			// 		: DateTime.MinValue,
			// 	IsRead = p.Item7
			// }).FirstOrDefault();
			var result = new DialogMessage
			{
				MessageId = dialogMessagesTuple.Item1,
				From = dialogMessagesTuple.Item3,
				To = dialogMessagesTuple.Item4,
				Text = dialogMessagesTuple.Item5,
				SentAt = DateTime.TryParse(dialogMessagesTuple.Item6, null, DateTimeStyles.RoundtripKind, out DateTime parsedDt)
					? parsedDt
					: DateTime.MinValue,
				IsRead = dialogMessagesTuple.Item7
			};
			
			return result;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error while retrieving message from Tarantool");
			throw;
		}
	}
	
	public async Task<bool> UpdateMessageReadStatusAsync(long messageId, bool isRead, CancellationToken cancellationToken)
	{
		try
		{
			var result = await Box.Call<TarantoolTuple<long, bool>, TarantoolTuple<bool>>(
				"mark_message_as_read",
				new TarantoolTuple<long, bool>(messageId, isRead)
			);

			return result.Data.Length > 0;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error while updating message read status in Tarantool");
			throw;
		}
	}
}