using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Dialog.Grpc.V1;
//using SocialNetwork.Dialog.Services;
using SocialNetworkApi.Services;
using SendMessageRequest = SocialNetwork.Domain.Models.Dialogs.SendMessageRequest;

namespace SocialNetworkApi.Controllers;

[ApiController]
[Route("api/v1/dialog")]
[Authorize]
public class DialogsV1Controller : BaseController
{
	private readonly GrpcDialogService.GrpcDialogServiceClient _grpcDialogServiceClient;

	public DialogsV1Controller(
		JwtTokenService jwtTokenService,
		GrpcClientFactory grpcClientFactory) : base(jwtTokenService)
	{
		_grpcDialogServiceClient = grpcClientFactory.CreateClient<GrpcDialogService.GrpcDialogServiceClient>("GrpcDialogServiceClientV1");
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

		//await _dialogService.SendMessageAsync(senderId, user_id, request.Text, cancellationToken);
		var sendMessageRequest = new SocialNetwork.Dialog.Grpc.V1.SendMessageRequest { SenderId = senderId, RecipientId = user_id, Text = request.Text };
		await _grpcDialogServiceClient.SendMessageAsync(sendMessageRequest);

		return Ok("Message sent successfully.");
	}

	/// <summary>
	/// Получение диалога между двумя пользователями
	/// </summary>
	[HttpGet("{user_id}/list")]
	public async Task<IActionResult> GetMessages(long user_id, CancellationToken cancellationToken)
	{
		var currentUserId = GetCurrentUserId();

		//var messages = await _dialogService.GetDialogAsync(currentUserId, user_id, cancellationToken);
		var sendMessageRequest = new SocialNetwork.Dialog.Grpc.V1.GetMessagesRequest { UserId1 = currentUserId, UserId2 = user_id };
		var sendMessageResponse = await _grpcDialogServiceClient.GetMessagesAsync(sendMessageRequest);
		var messages = sendMessageResponse.Messages;
		
		return Ok(messages);
	}
}