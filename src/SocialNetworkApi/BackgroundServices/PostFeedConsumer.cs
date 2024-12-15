using System.Text;
using Newtonsoft.Json;
using SocialNetwork.Application.Services;
using SocialNetwork.Domain.Entities;
using SocialNetworkApi.Hubs;

namespace SocialNetworkApi.BackgroundServices;

public class PostFeedConsumer : BackgroundService
{
	private readonly ILogger<PostFeedConsumer> _logger;
	private readonly RabbitMqService _rabbitMqService;
	private readonly PostFeedWebSocketService _webSocketService;

	public PostFeedConsumer(ILogger<PostFeedConsumer> logger, RabbitMqService rabbitMqService, PostFeedWebSocketService webSocketService)
	{
		_logger = logger;
		_rabbitMqService = rabbitMqService;
		_webSocketService = webSocketService;
	}

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		await _rabbitMqService.Consume(async (model, ea) =>
		{
			string userIdStr = null;
			string message = null;
			try
			{
				var routingKey = ea.RoutingKey;
				userIdStr = routingKey.Replace(_rabbitMqService.RoutingKeyPrefix, "");
				
				var body = ea.Body.ToArray();
				message = Encoding.UTF8.GetString(body);
				
				_logger.LogInformation($"Consume message to websocket for user={userIdStr}: {message}");
				
				var userId = long.Parse(userIdStr);
				var createdPost = JsonConvert.DeserializeObject<Post>(message);

				_logger.LogInformation($"Send message to websocket for user={userId}: {message}");

				if (userId <= 0 || createdPost == null)
				{
					_logger.LogError($"Invalid data. Can't send message to websocket userId='{userIdStr}', message={message}");
					return;
				}

				// Отправка уведомления через WebSocket
				await _webSocketService.SendPostToGroup(userId, createdPost);
			}
			catch (Exception ex)
			{
				_logger.LogError($"An error occured during consume userId='{userIdStr}', message={message}: {ex}");
			}
		}, cancellationToken);
	}
}