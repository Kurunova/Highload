using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Domain.Models.Posts;

public class CreatePostModel
{
	[Required(ErrorMessage = "Text is required.")]
	[StringLength(5000, ErrorMessage = "Text cannot exceed 5000 characters.")]
	public string Text { get; set; }
}
