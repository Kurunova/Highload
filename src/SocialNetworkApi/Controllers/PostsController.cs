using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Domain.DataAccess;
using SocialNetwork.Domain.Models.Posts;
using SocialNetwork.Domain.Services;
using SocialNetworkApi.Hubs;

namespace SocialNetworkApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : BaseController
{
    private readonly IPostService _postService;
    private readonly IUserRepository _userRepository;
    private readonly IHubContext<PostFeedHub> _hubContext;

    public PostsController(IPostService postService, IUserRepository userRepository, IHubContext<PostFeedHub> hubContext)
    {
        _postService = postService;
        _userRepository = userRepository;
        _hubContext = hubContext;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostModel post)
    {
        var currentUserId = GetCurrentUserId();
        var createdPost = await _postService.CreatePost(currentUserId, post.Text, CancellationToken.None);

        var subscribersId = await _userRepository.GetSubscriberIds(createdPost.AuthorUserId, CancellationToken.None);
        IReadOnlyList<string> subscribersIdReadOnly = subscribersId.Select(p => $"user-{p}").ToList();
        
        // Отправка уведомления через WebSocket
        var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<PostFeedHub>>();
        await hubContext.Clients.Groups(subscribersIdReadOnly).SendAsync("postFeedPosted", new
        {
            postId = createdPost.Id,
            postText = createdPost.Text,
            authorUserId = createdPost.AuthorUserId
        });
        
        return Ok(new { PostId = createdPost.Id });
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdatePost([FromBody] UpdatePostModel post)
    {
        var currentUserId = GetCurrentUserId();
        await _postService.UpdatePost(currentUserId, post.Id, post.Text, CancellationToken.None);
        return Ok(new { Message = "Post updated successfully." });
    }

    [Authorize]
    [HttpPut("delete/{id}")]
    public async Task<IActionResult> DeletePost(long id)
    {
        var currentUserId = GetCurrentUserId();
        await _postService.DeletePost(currentUserId, id, CancellationToken.None);
        return Ok(new { Message = "Post deleted successfully." });
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetPost(long id)
    {
        var post = await _postService.GetPostById(id, CancellationToken.None);
        return Ok(post);
    }

    [Authorize]
    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var currentUserId = GetCurrentUserId();
        var posts = await _postService.GetFeed(currentUserId, offset, limit, CancellationToken.None);
        return Ok(posts);
    }
}