using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SocialNetworkApi.Hubs;

[Authorize]
public class PostFeedHub : Hub
{
	private readonly ILogger<PostFeedHub> _logger;
	private readonly PostFeedWebSocketService _webSocketService;

	public PostFeedHub(ILogger<PostFeedHub> logger, PostFeedWebSocketService webSocketService)
	{
		_logger = logger;
		_webSocketService = webSocketService;
	}

	public override async Task OnConnectedAsync()
	{
		_logger.LogInformation($"Client connected: {Context.ConnectionId}");

		await _webSocketService.AddAuthorisedUserToWebsocketGroup(Context, Groups);
		
		await base.OnConnectedAsync();
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		_logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
		
		await _webSocketService.RemoveAuthorisedUserFromWebsocketGroup(Context, Groups);
		
		await base.OnDisconnectedAsync(exception);
	}
}