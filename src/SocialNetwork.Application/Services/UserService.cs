using SocialNetwork.Domain.DataAccess;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Models;
using SocialNetwork.Domain.Services;

namespace SocialNetwork.Application.Services;

public class UserService : IUserService
{
	private readonly IUserRepository _userRepository;

	public UserService(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<User> Create(CreateUser createUser, CancellationToken cancellationToken)
	{
		var registeredId = await _userRepository.Create(
			createUser.FirstName, 
			createUser.SecondName,
			createUser.Birthdate,
			createUser.Biography,
			createUser.City, 
			cancellationToken);

		var user = await _userRepository.GetById(registeredId, cancellationToken);
		
		return user;
	}

	public async Task<User> GetById(long id, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetById(id, cancellationToken);
		
		return user;
	}
}