using Microsoft.AspNetCore.Mvc;
using SocialNetworkApi.Services;

namespace SocialNetworkApi.Controllers;

public class BaseController : ControllerBase
{
	private readonly JwtTokenService _jwtTokenService;

	public BaseController(JwtTokenService jwtTokenService)
	{
		_jwtTokenService = jwtTokenService;
	}

	protected long GetCurrentUserId()
	{
		var userId = _jwtTokenService.GetCurrentUserId(User) 
		    ?? throw new UnauthorizedAccessException("User is not authenticated.");

		return userId;
	}
}