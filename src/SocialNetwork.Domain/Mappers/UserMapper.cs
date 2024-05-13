using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Models;

namespace SocialNetwork.Domain.Mappers;

public static class UserMapper
{
	public static UserInfo ToUserInfo(this User user)
	{
		return new UserInfo
		{
			Id = user.Id,
			FirstName = user.FirstName,
			LastName = user.LastName,
			Birthdate = user.Birthdate,
			Gender = user.Gender,
			City = user.City,
			Hobbies = user.Hobbies
		};
	}
}