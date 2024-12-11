using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Domain.Models.Dialogs;

public class SendMessageRequest
{
	[Required]
	public string Text { get; set; }
}