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

	public async Task<User> GetById(long id, CancellationToken cancellationToken)
	{
		var sql = "SELECT * FROM Users WHERE Id = @id";
		
		var user = await QuerySingleOrDefaultAsync<User>(sql, new
		{
			id = id
		}, cancellationToken);
		
		return user;
	}

	public async Task<long> Create(
		string firstName, 
		string secondName, 
		DateTime birthdate, 
		string biography, 
		string city, 
		CancellationToken cancellationToken)
	{
		string sql = @"INSERT INTO Users (FirstName, SecondName, Birthdate, Biography, City) VALUES (@firstName, @secondName, @birthdate, @biography, @city) RETURNING Id";
		
		var insertedId = await QuerySingleOrDefaultAsync<long>(sql, new
		{
			firstName = firstName,
			secondName = secondName,
			birthdate = birthdate,
			biography = biography,
			city = city
		}, cancellationToken);
		
		return insertedId;
	}

	public async Task Update(User user, CancellationToken cancellationToken)
	{
		string sql = @"UPDATE Users SET FirstName = @FirstName, SecondName = @SecondName, Birthdate = @Birthdate, Biography = @Biography, City = @City WHERE Id = @Id";
		
		await ExecuteAsync(sql, user, cancellationToken);
	}

	public async Task Delete(long id, CancellationToken cancellationToken)
	{
		string sql = @"DELETE FROM Users WHERE Id = @Id";
		
		await ExecuteAsync(sql, new
		{
			id = id
		}, cancellationToken);
	}
}