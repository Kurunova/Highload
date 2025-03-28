using System.Text.Json;
using Grpc.Core;
using SocialNetwork.Dialog.Services;
using SocialNetwork.Dialog.Grpc.V3;

namespace SocialNetwork.Dialog.Grpc.Services;

public class DialogServiceControllerV3 : GrpcDialogService.GrpcDialogServiceBase
{
	private readonly ILogger<DialogServiceControllerV3> _logger;
	private readonly IDialogService _dialogService;

	public DialogServiceControllerV3(ILogger<DialogServiceControllerV3> logger, IDialogService dialogService)
	{
		_logger = logger;
		_dialogService = dialogService;
	}

	public override async Task<SendMessageResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
	{
		var xRequestId = context.RequestHeaders.GetValue("X-Request-ID");
		_logger.LogInformation($"GrpcDialogService:V3:SendMessage: X-Request-ID {xRequestId}, request {JsonSerializer.Serialize(request)}");
		
		var result = await _dialogService.SendMessageAsync(request.SenderId, request.RecipientId, request.Text, context.CancellationToken);
		
		return new SendMessageResponse
		{
			Success = result
		};
	}
	
	public override async Task<GetMessagesResponse> GetMessages(GetMessagesRequest request, ServerCallContext context)
	{
		var version = context.RequestHeaders.GetValue("X-Request-ID");
		_logger.LogInformation($"GrpcDialogService:V3:GetMessages: X-Request-ID {version}, request {JsonSerializer.Serialize(request)}");
		
		var messages = await _dialogService.GetDialogAsync(request.UserId1, request.UserId2, context.CancellationToken);

		var response = new GetMessagesResponse();
		response.Messages.AddRange(messages.Select(p => new Message
		{
			MessageId = p.MessageId,
			From = p.From,
			To = p.To,
			Text = p.Text,
			SentAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
			IsRead = p.IsRead
		}));
		return response;
	}
	
	public override async Task<GetMessageByIdResponse> GetMessageById(GetMessageByIdRequest request, ServerCallContext context)
	{
		var xRequestId = context.RequestHeaders.GetValue("X-Request-ID");
		_logger.LogInformation($"GrpcDialogService:V3:GetMessageById: X-Request-ID {xRequestId}, request {JsonSerializer.Serialize(request)}");

		var message = await _dialogService.GetMessageByIdAsync(request.MessageId, context.CancellationToken);
		if (message == null)
		{
			throw new RpcException(new Status(StatusCode.NotFound, "Message not found"));
		}

		return new GetMessageByIdResponse
		{
			Message = new Message
			{
				MessageId = message.MessageId,
				From = message.From,
				To = message.To,
				Text = message.Text,
				SentAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(message.SentAt.ToUniversalTime()),
				IsRead = message.IsRead
			}
		};
	}
	
	public override async Task<MarkMessageAsReadResponse> MarkMessageAsRead(MarkMessageAsReadRequest request, ServerCallContext context)
	{
		var xRequestId = context.RequestHeaders.GetValue("X-Request-ID");
		_logger.LogInformation($"GrpcDialogService:V3:MarkMessageAsRead: X-Request-ID {xRequestId}, request {JsonSerializer.Serialize(request)}");
		
		var result = await _dialogService.MarkMessageAsReadAsync(request.MessageId, request.IsRead, context.CancellationToken);

		return new MarkMessageAsReadResponse
		{
			Success = result
		};
	}
}