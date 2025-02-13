﻿using SocialNetwork.Dialog.Entities;

namespace SocialNetwork.Dialog.DataAccess;

public interface IDialogRepository
{
	Task AddMessageAsync(DialogMessage message, CancellationToken cancellationToken);
	Task<IEnumerable<DialogMessage>> GetMessagesBetweenUsersAsync(long userId1, long userId2, CancellationToken cancellationToken);
}