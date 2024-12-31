using System.Text.Json;
using Grpc.Core;
using SocialNetwork.Dialog.Services;
using SocialNetwork.Dialog.Grpc.V1;

namespace SocialNetwork.Dialog.Grpc.Services;

public class DialogServiceControllerV1 : GrpcDialogService.GrpcDialogServiceBase
{
	private readonly ILogger<DialogServiceControllerV1> _logger;
	private readonly IDialogService _dialogService;

	public DialogServiceControllerV1(ILogger<DialogServiceControllerV1> logger, IDialogService dialogService)
	{
		_logger = logger;
		_dialogService = dialogService;
	}

	public override async Task<SendMessageResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
	{
		var version = context.RequestHeaders.GetValue("api-version");
		_logger.LogInformation($"GrpcDialogService:V1:SendMessage: api-version {version}, request {JsonSerializer.Serialize(request)}");
		
		var result = await _dialogService.SendMessageAsync(request.SenderId, request.RecipientId, request.Text, context.CancellationToken);
		
		return new SendMessageResponse
		{
			Success = result
		};
	}
	
	public override async Task<GetMessagesResponse> GetMessages(GetMessagesRequest request, ServerCallContext context)
	{
		var version = context.RequestHeaders.GetValue("api-version");
		_logger.LogInformation($"GrpcDialogService:V1:GetMessages: api-version {version}, request {JsonSerializer.Serialize(request)}");
		
		var messages = await _dialogService.GetDialogAsync(request.UserId1, request.UserId2, context.CancellationToken);

		var response = new GetMessagesResponse();
		response.Messages.AddRange(messages.Select(p => new Message
		{
			From = p.From,
			To = p.To,
			Text = p.Text,
			SentAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow)
		}));
		return response;
	}
}