using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Domain.Models;
using SocialNetwork.Domain.Services;
using SocialNetworkApi.Configurations;

namespace SocialNetworkApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
	private readonly JwtSettings _jwtSettings;
	private readonly IUserService _userService;
	
	public UsersController(IOptions<JwtSettings> jwtSettings, IUserService userService)
	{
		_jwtSettings = jwtSettings.Value;
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

		if (logined?.Id == null)
			return Unauthorized();

		var token = GenerateJwtToken(logined);
		return Ok(new { Token = token });
	}
	
	[Authorize]
	[HttpGet("get/{id}")]
	public async Task<IActionResult> GetUser(long id)
	{
		var user = await _userService.GetById(id, CancellationToken.None);
		return Ok(user);
	}
	
	private string GenerateJwtToken(UserInfo userInfo)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, userInfo.Id.ToString()),
			new(JwtRegisteredClaimNames.GivenName, userInfo.FirstName),
			new(JwtRegisteredClaimNames.FamilyName, userInfo.SecondName),
			new("birthdate", userInfo.Birthdate.ToString("yyyy-MM-dd")),
			new("city", userInfo.City),
			new("hobbies", userInfo.Hobbies)
		};
		
		var token = new JwtSecurityToken(
			_jwtSettings.Issuer,
			_jwtSettings.Audience,
			claims: claims,
			expires: DateTime.Now.AddSeconds(TimeSpan.FromSeconds(_jwtSettings.ExpirationSeconds).Seconds),
			signingCredentials: credentials);

		var jwtSecurityTokenHandler =  new JwtSecurityTokenHandler();
		var result = jwtSecurityTokenHandler.WriteToken(token);
		return result;
	}
}