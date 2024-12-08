using SocialNetwork.Domain.DataAccess;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Services;

namespace SocialNetwork.Application.Services;

public class DialogService : IDialogService
{
	private readonly IDialogRepository _dialogRepository;

	public DialogService(IDialogRepository dialogRepository)
	{
		_dialogRepository = dialogRepository;
	}

	public async Task SendMessageAsync(long senderId, long recipientId, string text, CancellationToken cancellationToken)
	{
		var message = new DialogMessage
		{
			From = senderId,
			To = recipientId,
			Text = text,
			SentAt = DateTime.UtcNow
		};

		await _dialogRepository.AddMessageAsync(message, cancellationToken);
	}

	public async Task<IEnumerable<DialogMessage>> GetDialogAsync(long userId1, long userId2, CancellationToken cancellationToken)
	{
		return await _dialogRepository.GetMessagesBetweenUsersAsync(userId1, userId2, cancellationToken);
	}
}