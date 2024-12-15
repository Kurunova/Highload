using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Application.Models;

public enum PostFeedOperation
{
	None,
	Created,
	Updated,
	Deleted
}

public class PostFeedMessage
{
	public PostFeedOperation Operation { get; set; }
	public Post Post { get; set; }
}