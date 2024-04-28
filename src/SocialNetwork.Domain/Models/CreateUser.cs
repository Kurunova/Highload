namespace SocialNetwork.Domain.Models;

public class CreateUser
{
	public string FirstName { get; set; }
	public string SecondName { get; set; }
	public DateTime Birthdate { get; set; }
	public string Biography { get; set; }
	public string City { get; set; }
}