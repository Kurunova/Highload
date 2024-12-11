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

	public override async Task OnConnectedAsync()
	{
		_logger.LogInformation($"Client connected: {Context.ConnectionId}");
		
		var userId = GetUserId(Context);
		_logger.LogInformation($"{Context.ConnectionId} : userId : {userId}");
		if (userId.HasValue)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
			_logger.LogInformation($"{Context.ConnectionId} : added to group userId : {userId}");
		}
		
		await base.OnConnectedAsync();
	}

	private long? GetUserId(HubCallerContext context)
	{
		var userIdClaim = context.User?.Claims?.FirstOrDefault(c => c.Type == "userId");
		if (context.User?.Identity?.IsAuthenticated == true 
		    && userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
		{
			return userId;
		}

		return null;
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		_logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
		
		var userId = Context.UserIdentifier;
		if (!string.IsNullOrEmpty(userId))
		{
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
			_logger.LogInformation($"{Context.ConnectionId} : removed from group userId : {userId}");
		}
		
		await base.OnDisconnectedAsync(exception);
	}
}