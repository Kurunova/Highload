using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SocialNetwork.Domain.DataAccess;
using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Mappers;
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

	public async Task<UserInfo> Create(CreateUser createUser, CancellationToken cancellationToken)
	{
		byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
		string passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password: createUser.Password,
			salt: salt,
			prf: KeyDerivationPrf.HMACSHA256,
			iterationCount: 100000,
			numBytesRequested: 256 / 8));
		
		var newUser = new User
		{
			Login = createUser.Login,
			PasswordHash = passwordHash,
			Salt = salt,
			FirstName = createUser.FirstName, 
			SecondName = createUser.SecondName,
			Birthdate = createUser.Birthdate,
			Gender = createUser.Gender,
			City = createUser.City, 
			Hobbies = createUser.Hobbies
		};
		
		var registeredId = await _userRepository.Create(newUser, cancellationToken);

		var user = await _userRepository.GetById(registeredId, cancellationToken);
		
		return user.ToUserInfo();
	}
	
	public async Task<UserInfo> Login(LoginUser loginUser, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetByLogin(loginUser.Login, cancellationToken);
		
		string passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password: loginUser.Password,
			salt: user.Salt,
			prf: KeyDerivationPrf.HMACSHA256,
			iterationCount: 100000,
			numBytesRequested: 256 / 8));

		if (user.PasswordHash != passwordHash)
			return null;
		
		return user.ToUserInfo();
	}

	public async Task<UserInfo> GetById(long id, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetById(id, cancellationToken);
		
		return user.ToUserInfo();
	}
}