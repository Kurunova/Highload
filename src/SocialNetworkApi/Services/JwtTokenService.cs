using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Domain.Models;
using SocialNetworkApi.Configurations;

namespace SocialNetworkApi.Services;

public class JwtTokenService 
{
	private readonly JwtSettings _jwtSettings;

	public JwtTokenService(IOptions<JwtSettings> jwtSettings)
	{
		_jwtSettings = jwtSettings.Value;
	}

	public string GenerateJwtToken(UserInfo userInfo)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, userInfo.Id.ToString()),
			new(JwtRegisteredClaimNames.GivenName, userInfo.FirstName),
			new(JwtRegisteredClaimNames.FamilyName, userInfo.LastName),
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