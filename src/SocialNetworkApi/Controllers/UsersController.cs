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

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginUser user)
	{
		var logined = await _userService.Login(user, CancellationToken.None);
		
		// var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
		// var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
		//
		// var jwtSecurityToken = new JwtSecurityToken(_config["Jwt:Issuer"],
		// 	_config["Jwt:Issuer"],
		// 	null,
		// 	expires: DateTime.Now.AddMinutes(120),
		// 	signingCredentials: credentials);
		//
		// var token =  new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

		return CreatedAtAction(nameof(Register), new { userId = logined.Id });
	}
	
	//[Authorize]
	[HttpGet("get/{id}")]
	public async Task<IActionResult> GetUser(long id)
	{
		var user = await _userService.GetById(id, CancellationToken.None);
		return Ok(user);
	}
}