using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Domain.Models;
using SocialNetwork.Domain.Services;
using SocialNetworkApi.Services;

namespace SocialNetworkApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : BaseController
{
	private readonly JwtTokenService _jwtTokenService;
	private readonly IUserService _userService;
	
	public UsersController(JwtTokenService jwtTokenService, IUserService userService) : base(jwtTokenService)
	{
		_jwtTokenService = jwtTokenService;
		_userService = userService;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] CreateUser user)
	{
		var registered = await _userService.Create(user, CancellationToken.None);
		return CreatedAtAction(nameof(Register), new { userId = registered?.Id });
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginUser user)
	{
		var logined = await _userService.Login(user, CancellationToken.None);

		if (logined?.Id == null)
			return Unauthorized();

		var token = _jwtTokenService.GenerateJwtToken(logined);
		return Ok(new { Token = token });
	}
	
	[Authorize]
	[HttpGet("get/{id}")]
	public async Task<IActionResult> GetUser(long id)
	{
		var user = await _userService.GetById(id, CancellationToken.None);
		return Ok(user);
	}
	
	[HttpGet("search")]
	public async Task<IActionResult> Search([FromQuery] string firstName, [FromQuery] string lastName)
	{
		var users = await _userService.Search(new SearchUser { FirstName = firstName, LastName = lastName }, CancellationToken.None);
		if (!users.Any())
			return Ok("No users found matching the criteria.");

		return Ok(users);
	}
	
	[Authorize]
	[HttpPut("friend/set/{userId}")]
	public async Task<IActionResult> AddFriend(long userId)
	{
		var currentUserId = GetCurrentUserId();
		await _userService.AddFriend(currentUserId, userId, CancellationToken.None);
		return Ok(new { Message = "Friend added successfully." });
	}

	[Authorize]
	[HttpPut("friend/delete/{userId}")]
	public async Task<IActionResult> RemoveFriend(long userId)
	{
		var currentUserId = GetCurrentUserId();
		await _userService.RemoveFriend(currentUserId, userId, CancellationToken.None);
		return Ok(new { Message = "Friend removed successfully." });
	}
}