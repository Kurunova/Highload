using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.Services;

public interface IDialogService
{
	Task SendMessageAsync(long senderId, long recipientId, string text, CancellationToken cancellationToken);
	Task<IEnumerable<DialogMessage>> GetDialogAsync(long userId1, long userId2, CancellationToken cancellationToken);
}