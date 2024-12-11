using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SocialNetworkApi.Hubs;

[Authorize]
public class PostFeedHub : Hub
{
	private readonly ILogger<PostFeedHub> _logger;

	public PostFeedHub(ILogger<PostFeedHub> logger)
	{
		_logger = logger;
	}

	public override Task OnConnectedAsync()
	{
		_logger.LogInformation($"Client connected: {Context.ConnectionId}");
		return base.OnConnectedAsync();
	}

	public override Task OnDisconnectedAsync(Exception? exception)
	{
		_logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
		return base.OnDisconnectedAsync(exception);
	}
}