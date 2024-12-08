using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Domain.Models.Dialogs;
using SocialNetwork.Domain.Services;

namespace SocialNetworkApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DialogsController : BaseController
{
	private readonly IDialogService _dialogService;

	public DialogsController(IDialogService dialogService)
	{
		_dialogService = dialogService;
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

		await _dialogService.SendMessageAsync(senderId, user_id, request.Text, cancellationToken);

		return Ok("Message sent successfully.");
	}

	/// <summary>
	/// Получение диалога между двумя пользователями
	/// </summary>
	[HttpGet("{user_id}/list")]
	public async Task<IActionResult> GetMessages(long user_id, CancellationToken cancellationToken)
	{
		var currentUserId = GetCurrentUserId();

		var messages = await _dialogService.GetDialogAsync(currentUserId, user_id, cancellationToken);

		return Ok(messages);
	}
}