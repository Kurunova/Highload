namespace SocialNetworkApi.Configurations;

public class JwtSettings
{
	public string Issuer { get; set; }
	public string Key { get; set; }
	public string Audience { get; set; }
	public long ExpirationSeconds { get; set; } = 86400; // 24 hours
}