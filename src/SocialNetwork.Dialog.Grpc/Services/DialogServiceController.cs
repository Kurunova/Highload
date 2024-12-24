using Grpc.Core;
using SocialNetwork.Dialog.Services;

namespace SocialNetwork.Dialog.Grpc.Services;

public class DialogServiceController : GrpcDialogService.GrpcDialogServiceBase
{
	private readonly ILogger<DialogServiceController> _logger;
	private readonly IDialogService _dialogService;

	public DialogServiceController(ILogger<DialogServiceController> logger, IDialogService dialogService)
	{
		_logger = logger;
		_dialogService = dialogService;
	}

	public override async Task<SendMessageResponse> SendMessage(SendMessageRequest request, ServerCallContext context)
	{
		var result = await _dialogService.SendMessageAsync(request.SenderId, request.RecipientId, request.Text, context.CancellationToken);
		
		return new SendMessageResponse
		{
			Success = result
		};
	}
	
	public override async Task<GetMessagesResponse> GetMessages(GetMessagesRequest request, ServerCallContext context)
	{
		var messages = await _dialogService.GetDialogAsync(request.UserId1, request.UserId2, context.CancellationToken);

		var response = new GetMessagesResponse();
		response.Messages.AddRange(messages.Select(p => new SocialNetwork.Dialog.Grpc.Message
		{
			From = p.From,
			To = p.To,
			Text = p.Text,
			SentAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow)
		}));
		return response;
	}
}