using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Domain.Models;
using SocialNetwork.Domain.Services;

namespace SocialNetworkApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
	private readonly IUserService _userService;
	
	public UsersController(IUserService userService)
	{
		_userService = userService;
	}
	
	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] CreateUser user)
	{
		var registered = await _userService.Create(user, CancellationToken.None);
		return CreatedAtAction(nameof(Register), new { userId = registered.Id });
	}

	[HttpGet("get/{id}")]
	public async Task<IActionResult> GetUser(long id)
	{
		var user = await _userService.GetById(id, CancellationToken.None);
		return Ok(user);
	}
}