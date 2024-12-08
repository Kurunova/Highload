namespace SocialNetwork.Domain.Entities;

public class Post
{
	public long Id { get; set; }
	public long AuthorUserId { get; set; }
	public string Text { get; set; }
	public DateTime CreatedAt { get; set; }
}