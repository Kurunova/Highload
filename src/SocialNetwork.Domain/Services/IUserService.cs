using SocialNetwork.Domain.Models;

namespace SocialNetwork.Domain.Services;

public interface IUserService
{
	Task<UserInfo> Create(CreateUser createUser, CancellationToken cancellationToken);
	Task<UserInfo> GetById(long id, CancellationToken cancellationToken);
	Task<UserInfo> Login(LoginUser user, CancellationToken cancellationToken);
	Task<UserInfo[]> Search(SearchUser searchUser, CancellationToken cancellationToken);
}