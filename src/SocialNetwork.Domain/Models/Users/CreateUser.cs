using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.Models;

public class CreateUser
{
	public string Login { get; set; }
	public string Password { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public DateTime Birthdate { get; set; }
	public GenderType Gender { get; set; }
	public string City { get; set; }
	public string Hobbies { get; set; }
}