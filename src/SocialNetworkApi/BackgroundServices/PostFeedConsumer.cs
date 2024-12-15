using System.Text;
using Newtonsoft.Json;
using SocialNetwork.Application.Models;
using SocialNetwork.Application.Services;
using SocialNetwork.Domain.DataAccess;
using SocialNetworkApi.Hubs;

namespace SocialNetworkApi.BackgroundServices;

public class PostFeedConsumer : BackgroundService
{
	private readonly ILogger<PostFeedConsumer> _logger;
	private readonly RabbitMqService _rabbitMqService;
	private readonly PostFeedWebSocketService _webSocketService;
	private readonly PostCacheService _postCacheService;
	private readonly IUserRepository _userRepository;

	public PostFeedConsumer(
		ILogger<PostFeedConsumer> logger, 
		RabbitMqService rabbitMqService, 
		PostFeedWebSocketService webSocketService, 
		PostCacheService postCacheService, 
		IUserRepository userRepository)
	{
		_logger = logger;
		_rabbitMqService = rabbitMqService;
		_webSocketService = webSocketService;
		_postCacheService = postCacheService;
		_userRepository = userRepository;
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
				var postFeedMessage = JsonConvert.DeserializeObject<PostFeedMessage>(message);

				if (userId <= 0 || postFeedMessage?.Post == null)
				{
					_logger.LogError($"Invalid data. Can't send message to websocket userId='{userIdStr}', message={message}");
					return;
				}

				var post = postFeedMessage.Post;

				var subscribersId = await _userRepository.GetSubscriberIds(userId, CancellationToken.None);
				foreach (var subscriberId in subscribersId)
				{
					switch (postFeedMessage.Operation)
					{
						case PostFeedOperation.Created:
							// Отправка уведомления через WebSocket
							_logger.LogInformation($"Send message to websocket for user={subscriberId}: {message}");
							await _webSocketService.SendPostToGroup(subscriberId, post);
							
							// Добавляем в кеш
							_logger.LogInformation($"Append post to cache for user={subscriberId}: {message}");
							await _postCacheService.AppendPostToFeedInCache(subscriberId, post, post.CreatedAt.Ticks);
							break;
						case PostFeedOperation.Updated:
							// Обновляем в кеше
							_logger.LogInformation($"Update post in cache for user={subscriberId}: {message}");
							await _postCacheService.UpdateFeedInCache(subscriberId, post);
							break;
						case PostFeedOperation.Deleted:
							// Удаляем из кеша
							_logger.LogInformation($"Delete post in cache for user={subscriberId}: {message}");
							await _postCacheService.RemovePostFromFeedInCache(subscriberId, post.Id);
							break;
						default:
							_logger.LogWarning($"No supported operation for user={subscriberId}: {message}");
							break;
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"An error occured during consume userId='{userIdStr}', message={message}: {ex}");
			}
		}, cancellationToken);
	}
}