using Microsoft.Extensions.Logging;
using SocialNetwork.Domain.DataAccess;
using SocialNetwork.Domain.Entities;

namespace SocialNetwork.DataAccess.DataAccess;

public class UserRepository : BaseRepository, IUserRepository
{
	public UserRepository(ILoggerFactory loggerFactory, IPostgresConnectionFactory connectionFactory) 
		: base(loggerFactory, connectionFactory)
	{
	}

	public async Task<User> GetByLogin(string login, CancellationToken cancellationToken)
	{
		var sql = "SELECT * FROM Users WHERE Login = @login";
		
		var user = await QuerySingleOrDefaultAsync<User>(sql, new
		{
			login = login
		}, cancellationToken);
		
		return user;
	}

	public async Task<User> GetById(long id, CancellationToken cancellationToken)
	{
		var sql = "SELECT * FROM Users WHERE Id = @id";
		
		var user = await QuerySingleOrDefaultAsync<User>(sql, new
		{
			id = id
		}, cancellationToken);
		
		return user;
	}

	public async Task<User[]> Search(string firstName, string lastName, CancellationToken cancellationToken)
	{
		var sql = "SELECT * " +
		          "FROM Users " +
		          "WHERE FirstName ILIKE @firstName || '%' and LastName ILIKE @lastName || '%'" +
		          "ORDER BY Id";
		
		var users = await QueryAsync<User>(sql, new
		{
			firstName = firstName,
			lastName = lastName
		}, cancellationToken);
		
		return users.ToArray();
	}

	public async Task<User> Create(User user, CancellationToken cancellationToken)
	{
		string sql = @"
			INSERT INTO Users (Login, PasswordHash, Salt, FirstName, LastName, Birthdate, Gender, City, Hobbies) 
			VALUES (@login, @passwordHash, @salt, @firstName, @lastName, @birthdate, CAST(@gender as public.gender_type), @city, @hobbies) 
			RETURNING Id, Login, PasswordHash, Salt, FirstName, LastName, Birthdate, Gender, City, Hobbies";
		
		var insertedUser = await ExecuteAsync<User>(sql, new
		{
			login = user.Login,
			passwordHash = user.PasswordHash,
			salt = user.Salt,
			firstName = user.FirstName,
			lastName = user.LastName,
			birthdate = user.Birthdate,
			gender = user.Gender.ToString(),
			city = user.City,
			hobbies = user.Hobbies
		}, cancellationToken);
		
		_logger.LogInformation($"User created: {insertedUser?.Id}");
		
		return insertedUser;
	}

	public async Task Update(User user, CancellationToken cancellationToken)
	{
		string sql = @"UPDATE Users 
			SET FirstName = @firstName, 
			    LastName = @lastName, 
			    Birthdate = @birthdate, 
			    Gender = CAST(@gender as public.gender_type), 
			    City = @city,
			    Hobbies = @hobbies
			WHERE Id = @id";
		
		await ExecuteAsync(sql, new
		{
			firstName = user.FirstName,
			lastName = user.LastName,
			birthdate = user.Birthdate,
			gender = user.Gender.ToString(),
			city = user.City,
			hobbies = user.Hobbies
		}, cancellationToken);
	}

	public async Task Delete(long id, CancellationToken cancellationToken)
	{
		string sql = @"DELETE FROM Users WHERE Id = @id";
		
		await ExecuteAsync(sql, new
		{
			id = id
		}, cancellationToken);
	}
	
	public async Task AddFriend(long userId, long friendId, CancellationToken cancellationToken)
	{
		var sql = @"
        INSERT INTO UserFriends (UserId, FriendId)
        VALUES (@userId, @friendId)
        ON CONFLICT DO NOTHING";

		await ExecuteAsync(sql, new { userId, friendId }, cancellationToken);
	}

	public async Task RemoveFriend(long userId, long friendId, CancellationToken cancellationToken)
	{
		var sql = @"
        DELETE FROM UserFriends 
        WHERE UserId = @userId AND FriendId = @friendId";

		await ExecuteAsync(sql, new { userId, friendId }, cancellationToken);
	}
	
	public async Task<List<long>> GetFriendIds(long userId, CancellationToken cancellationToken)
	{
		var sql = @"
        SELECT FriendId
        FROM UserFriends
        WHERE UserId = @userId";

		var friendIds = await QueryAsync<long>(sql, new { userId }, cancellationToken);
		return friendIds.ToList();
	}
	
	public async Task<List<long>> GetSubscriberIds(long publisherId, CancellationToken cancellationToken)
	{
		var sql = @"
        SELECT UserId
        FROM UserFriends
        WHERE FriendId = @publisherId";

		var friendIds = await QueryAsync<long>(sql, new { publisherId }, cancellationToken);
		return friendIds.ToList();
	}
}