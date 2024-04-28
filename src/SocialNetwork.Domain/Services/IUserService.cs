using SocialNetwork.Domain.Entities;
using SocialNetwork.Domain.Models;

namespace SocialNetwork.Domain.Services;

public interface IUserService
{
	Task<User> Create(CreateUser createUser, CancellationToken cancellationToken);
	Task<User> GetById(long id, CancellationToken cancellationToken);
}