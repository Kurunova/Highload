using SocialNetwork.Domain.Entities;

namespace SocialNetwork.Domain.DataAccess;

public interface IUserRepository
{
	Task<long> Create(
		string firstName, 
		string secondName, 
		DateTime birthdate, 
		string biography, 
		string city, 
		CancellationToken cancellationToken);
	Task<User> GetById(long id, CancellationToken cancellationToken);
	Task Update(User user, CancellationToken cancellationToken);
	Task Delete(long id, CancellationToken cancellationToken);
}