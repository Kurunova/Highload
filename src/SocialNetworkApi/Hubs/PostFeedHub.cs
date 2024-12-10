using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SocialNetworkApi.Hubs;

[Authorize]
public class PostFeedHub : Hub
{
	public async Task SendPostUpdate(string postId, string postText, string authorUserId)
	{
		// Отправить событие всем подписанным клиентам
		await Clients.All.SendAsync("postFeedPosted", new
		{
			postId,
			postText,
			authorUserId
		});
	}
}