using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Domain.Models.Posts;
using SocialNetwork.Domain.Services;
using SocialNetworkApi.Services;

namespace SocialNetworkApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : BaseController
{
    private readonly IPostService _postService;

    public PostsController(JwtTokenService jwtTokenService, IPostService postService) : base(jwtTokenService)
    {
        _postService = postService;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostModel post)
    {
        var currentUserId = GetCurrentUserId();
        var createdPost = await _postService.CreatePost(currentUserId, post.Text, CancellationToken.None);
        
        // Отправка уведомления через WebSocket
        // var subscribersId = await _userRepository.GetSubscriberIds(createdPost.AuthorUserId, CancellationToken.None);
        // await _webSocketService.SendPostToGroup(subscribersId, createdPost);
        
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