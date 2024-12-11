using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.DataAccess;

public interface IDialogRepository
{
	Task AddMessageAsync(DialogMessage message, CancellationToken cancellationToken);
	Task<IEnumerable<DialogMessage>> GetMessagesBetweenUsersAsync(long userId1, long userId2, CancellationToken cancellationToken);
}