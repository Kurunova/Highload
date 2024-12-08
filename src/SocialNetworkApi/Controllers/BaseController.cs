using Microsoft.AspNetCore.Mvc;

namespace SocialNetworkApi.Controllers;

public class BaseController : ControllerBase
{
	protected long GetCurrentUserId()
	{
		var userIdClaim = User.Claims?.FirstOrDefault(c => c.Type == "userId");
		if (User.Identity?.IsAuthenticated == true 
		    && userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
		{
			return userId;
		}
        
		throw new UnauthorizedAccessException("User is not authenticated.");
	}
}