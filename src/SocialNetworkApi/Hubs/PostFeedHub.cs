using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace SocialNetworkApi.Hubs;

//[Authorize]
public class PostFeedHub : Hub
{
	protected readonly ILogger<PostFeedHub> _logger;
	
	// Хранилище подключенных клиентов
	private static ConcurrentDictionary<string, string> ConnectedClients = new ConcurrentDictionary<string, string>();

	public PostFeedHub(ILogger<PostFeedHub> logger)
	{
		_logger = logger;
	}

	public override Task OnConnectedAsync()
	{
		// Добавляем подключение клиента в список
		ConnectedClients.TryAdd(Context.ConnectionId, Context.User?.Identity?.Name ?? "Anonymous");
		_logger.LogInformation($"Client connected: {Context.ConnectionId}");
		return base.OnConnectedAsync();
	}

	public override Task OnDisconnectedAsync(Exception? exception)
	{
		// Удаляем клиента из списка
		ConnectedClients.TryRemove(Context.ConnectionId, out _);
		_logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
		return base.OnDisconnectedAsync(exception);
	}

	// Метод для проверки, есть ли подключенные клиенты
	public Task<int> GetConnectedClientsCount()
	{
		return Task.FromResult(ConnectedClients.Count);
	}
	
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