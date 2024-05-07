using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.Models;

public class UserInfo
{
	public long Id { get; set; }
	public string FirstName { get; set; }
	public string SecondName { get; set; }
	public DateTime Birthdate { get; set; }
	public GenderType Gender { get; set; }
	public string City { get; set; }
	public string Hobbies { get; set; }
}