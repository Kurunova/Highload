using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.DataAccess;

public interface IUserRepository
{
	Task<User> GetByLogin(string login, CancellationToken cancellationToken);
	Task<User> GetById(long id, CancellationToken cancellationToken);
	Task<User[]> Search(string firstName, string lastName, CancellationToken cancellationToken);
	Task<User> Create(User user, CancellationToken cancellationToken);
	Task Update(User user, CancellationToken cancellationToken);
	Task Delete(long id, CancellationToken cancellationToken);
	Task AddFriend(long userId, long friendId, CancellationToken cancellationToken);
	Task RemoveFriend(long userId, long friendId, CancellationToken cancellationToken);
	Task<List<long>> GetFriendIds(long userId, CancellationToken cancellationToken);
}