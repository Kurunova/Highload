using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Domain.Entities;
using SocialNetworkApi.Services;

namespace SocialNetworkApi.Hubs;

public class PostFeedWebSocketService
{
	private readonly ILogger<PostFeedWebSocketService> _logger;
	private readonly JwtTokenService _jwtTokenService;
	private readonly IHubContext<PostFeedHub> _hubContext;
	private readonly string _groupNameFormat = "user-{0}";
	
	public PostFeedWebSocketService(ILogger<PostFeedWebSocketService> logger, JwtTokenService jwtTokenService, IHubContext<PostFeedHub> hubContext)
	{
		_logger = logger;
		_jwtTokenService = jwtTokenService;
		_hubContext = hubContext;
	}
	
	public async Task SendPostToGroup(long userId, Post post)
	{
		var groupName = string.Format(_groupNameFormat, userId);
		await _hubContext.Clients.Groups(groupName).SendAsync("postFeedPosted", new
		{
			postId = post.Id,
			postText = post.Text,
			authorUserId = post.AuthorUserId
		});
	}
	
	public async Task SendPostToGroup(List<long> subscribersId, Post post)
	{
		IReadOnlyList<string> subscribersIdReadOnly = subscribersId.Select(p => string.Format(_groupNameFormat, p)).ToList();
		
		await _hubContext.Clients.Groups(subscribersIdReadOnly).SendAsync("postFeedPosted", new
		{
			postId = post.Id,
			postText = post.Text,
			authorUserId = post.AuthorUserId
		});
	}
	
	public async Task AddAuthorisedUserToWebsocketGroup(HubCallerContext context, IGroupManager groups)
	{
		var userId = _jwtTokenService.GetCurrentUserId(context.User);
		_logger.LogInformation($"{context.ConnectionId} : userId : {userId}");
		if (userId.HasValue)
		{
			await groups.AddToGroupAsync(context.ConnectionId, string.Format(_groupNameFormat, userId));
			_logger.LogInformation($"{context.ConnectionId} : added to group userId : {userId}");
		}
	}
	
	public async Task RemoveAuthorisedUserFromWebsocketGroup(HubCallerContext context, IGroupManager groups)
	{
		var userId = context.UserIdentifier;
		if (!string.IsNullOrEmpty(userId))
		{
			await groups.RemoveFromGroupAsync(context.ConnectionId, string.Format(_groupNameFormat, userId));
			_logger.LogInformation($"{context.ConnectionId} : removed from group userId : {userId}");
		}
	}
}