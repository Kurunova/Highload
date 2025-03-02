using SocialNetwork.Dialog.Entities;

namespace SocialNetwork.Dialog.Services;

public interface IDialogService
{
	Task<bool> SendMessageAsync(long senderId, long recipientId, string text, CancellationToken cancellationToken);
	Task<IEnumerable<DialogMessage>> GetDialogAsync(long userId1, long userId2, CancellationToken cancellationToken);
	Task<DialogMessage?> GetMessageByIdAsync(long messageId, CancellationToken cancellationToken);
	Task<bool> MarkMessageAsReadAsync(long messageId, bool isRead, CancellationToken cancellationToken);
}