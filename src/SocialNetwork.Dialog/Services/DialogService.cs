﻿using SocialNetwork.Dialog.DataAccess;
using SocialNetwork.Dialog.Entities;

namespace SocialNetwork.Dialog.Services;

public class DialogService : IDialogService
{
	private readonly IDialogRepository _dialogRepository;

	public DialogService(IDialogRepository dialogRepository)
	{
		_dialogRepository = dialogRepository;
	}

	public async Task<bool> SendMessageAsync(long senderId, long recipientId, string text, CancellationToken cancellationToken)
	{
		var message = new DialogMessage
		{
			From = senderId,
			To = recipientId,
			Text = text,
			SentAt = DateTime.UtcNow
		};

		await _dialogRepository.AddMessageAsync(message, cancellationToken);
		return true;
	}

	public async Task<IEnumerable<DialogMessage>> GetDialogAsync(long userId1, long userId2, CancellationToken cancellationToken)
	{
		return await _dialogRepository.GetMessagesBetweenUsersAsync(userId1, userId2, cancellationToken);
	}
	
	public async Task<DialogMessage?> GetMessageByIdAsync(long messageId, CancellationToken cancellationToken)
	{
		return await _dialogRepository.GetMessageByIdAsync(messageId, cancellationToken);
	}
	
	public async Task<bool> MarkMessageAsReadAsync(long messageId, bool isRead, CancellationToken cancellationToken)
	{
		return await _dialogRepository.UpdateMessageReadStatusAsync(messageId, isRead, cancellationToken);
	}
}