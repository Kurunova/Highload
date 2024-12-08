using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Domain.Models.Posts;

public class UpdatePostModel
{
	[Required(ErrorMessage = "Id is required.")]
	public long Id { get; set; }

	[Required(ErrorMessage = "Text is required.")]
	[StringLength(5000, ErrorMessage = "Text cannot exceed 5000 characters.")]
	public string Text { get; set; }
}