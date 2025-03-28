using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Counters.Grpc.V1;
using SocialNetwork.Dialog.Grpc.V3;
using SocialNetworkApi.Services;
using SendMessageRequest = SocialNetwork.Domain.Models.Dialogs.SendMessageRequest;

namespace SocialNetworkApi.Controllers;

[ApiController]
[Route("api/v3/dialog")]
[Authorize]
public class DialogsV3Controller : BaseController
{
	private readonly ILogger<DialogsV3Controller> _logger;
	private readonly GrpcDialogService.GrpcDialogServiceClient _grpcDialogServiceClient;
	private readonly GrpcCounterService.GrpcCounterServiceClient _grpcCountersServiceClient;

	public DialogsV3Controller(
		ILogger<DialogsV3Controller> logger,
		JwtTokenService jwtTokenService,
		GrpcClientFactory grpcClientFactory) : base(jwtTokenService)
	{
		_logger = logger;
		_grpcDialogServiceClient = grpcClientFactory.CreateClient<GrpcDialogService.GrpcDialogServiceClient>("GrpcDialogServiceClientV3");
		_grpcCountersServiceClient = grpcClientFactory.CreateClient<GrpcCounterService.GrpcCounterServiceClient>("GrpcCounterServiceClientV1");
	}

	/// <summary>
	/// Отправка сообщения пользователю
	/// </summary>
	[HttpPost("{user_id}/send")]
	public async Task<IActionResult> SendMessage(long user_id, [FromBody] SendMessageRequest request, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request.Text))
			return BadRequest("Message text cannot be empty.");

		var senderId = GetCurrentUserId();

		// Обновляем счетчик непрочитанных сообщений (Saga шаг)
		var incrementUnreadMessagesRequest = new IncrementUnreadMessagesCountRequest { UserId = user_id, Count = 1 };
		var incrementUnreadMessagesResponse = await _grpcCountersServiceClient.IncrementUnreadMessagesCountAsync(incrementUnreadMessagesRequest);
		if (!incrementUnreadMessagesResponse.Success)
		{
			_logger.LogError($"Failed to mark message for userId {user_id} as read.");
			return StatusCode(500, "Failed.");
		}

		try
		{
			//await _dialogService.SendMessageAsync(senderId, user_id, request.Text, cancellationToken);
			var sendMessageRequest = new SocialNetwork.Dialog.Grpc.V3.SendMessageRequest { SenderId = senderId, RecipientId = user_id, Text = request.Text };
			await _grpcDialogServiceClient.SendMessageAsync(sendMessageRequest);
			
			return Ok("Message sent successfully.");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Send message was failed for userId {user_id}.");
			
			// Откат обновления количества непрочитанных сообщений
			var decrementUnreadMessagesRequest = new DecrementUnreadMessagesCountRequest { UserId = user_id, Count = 1 };
			var decrementUnreadMessagesResponse = await _grpcCountersServiceClient.DecrementUnreadMessagesCountAsync(decrementUnreadMessagesRequest);
			
			return StatusCode(500, "Failed.");
		}
	}

	/// <summary>
	/// Получение диалога между двумя пользователями
	/// </summary>
	[HttpGet("{user_id}/list")]
	public async Task<IActionResult> GetMessages(long user_id, CancellationToken cancellationToken)
	{
		var currentUserId = GetCurrentUserId();

		var sendMessageRequest = new SocialNetwork.Dialog.Grpc.V3.GetMessagesRequest { UserId1 = currentUserId, UserId2 = user_id };
		var sendMessageResponse = await _grpcDialogServiceClient.GetMessagesAsync(sendMessageRequest);
		var messages = sendMessageResponse.Messages;
		
		return Ok(messages);
	}
	
	/// <summary>
	/// Помечает сообщение как прочитанное и обновляет счетчик с использованием Saga
	/// </summary>
	[HttpPost("mark-as-read/{messageId}")]
	public async Task<IActionResult> GetMessage(long messageId, CancellationToken cancellationToken)
	{
		_logger.LogInformation($"Starting Saga to mark message {messageId} as read.");
	
		// Получаем сообщение и его владельца
		var getMessageByIdRequest = new GetMessageByIdRequest { MessageId = messageId };
		var getMessageByIdResponse = await _grpcDialogServiceClient.GetMessageByIdAsync(getMessageByIdRequest);
		if (getMessageByIdResponse?.Message == null)
		{
			_logger.LogWarning($"Message {messageId} not found.");
			return NotFound("Message not found");
		}
		
		// Проверяем, было ли сообщение уже прочитано
		if (getMessageByIdResponse.Message.IsRead)
		{
			_logger.LogInformation($"Message {messageId} is already marked as read.");
			return Ok("Message already marked as read.");
		}
		
		// Обновляем статус прочитанности
		var markMessageAsReadRequest = new MarkMessageAsReadRequest { MessageId = messageId, IsRead = true };
		var markMessageAsReadResponse = await _grpcDialogServiceClient.MarkMessageAsReadAsync(markMessageAsReadRequest);
		if (!markMessageAsReadResponse.Success)
		{
			_logger.LogError($"Failed to mark message {messageId} as read.");
			return StatusCode(500, "Failed to mark message as read.");
		}
	
		try
		{
			// Откат обновления количества непрочитанных сообщений
			var decrementUnreadMessagesRequest = new DecrementUnreadMessagesCountRequest { UserId = getMessageByIdResponse.Message.To, Count = 1 };
			var decrementUnreadMessagesResponse = await _grpcCountersServiceClient.DecrementUnreadMessagesCountAsync(decrementUnreadMessagesRequest);
			
			_logger.LogInformation($"Saga completed: Message {messageId} marked as read, counter updated.");
			return Ok("Message marked as read.");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Saga failed: Message {messageId} was marked as read, but counter update failed.");
			
			// Откат — отменяем обновление статуса прочитанности
			var markMessageAsUnReadRequest = new MarkMessageAsReadRequest { MessageId = messageId, IsRead = false };
			var markMessageAsUnReadResponse = await _grpcDialogServiceClient.MarkMessageAsReadAsync(markMessageAsUnReadRequest);
			return StatusCode(500, "Failed to update counter. Message status rolled back.");
		}
	}
}