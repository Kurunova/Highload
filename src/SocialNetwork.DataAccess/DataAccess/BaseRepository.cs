using Dapper;
using Microsoft.Extensions.Logging;
using SocialNetwork.Domain.DataAccess;

namespace SocialNetwork.DataAccess.DataAccess;

public class BaseRepository
{
	private readonly ILogger<BaseRepository> _logger;
	protected readonly IPostgresConnectionFactory ConnectionFactory;

	protected BaseRepository(ILoggerFactory loggerFactory, IPostgresConnectionFactory connectionFactory)
	{
		_logger = loggerFactory.CreateLogger<BaseRepository>();
		ConnectionFactory = connectionFactory;
	}
	
	protected Task<IEnumerable<T>> QueryAsync<T>(string sql, CancellationToken cancellationToken) 
		=> QueryAsync<T>(sql, null, cancellationToken);

	protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters, CancellationToken cancellationToken)
	{
		try
		{
			await using var connection = ConnectionFactory.CreateConnection();
			await connection.OpenAsync(cancellationToken);
		
			var result = await connection.QueryAsync<T>(sql, parameters);

			await connection.CloseAsync();
			return result;
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Error querying data from db");
			throw;
		}
	}
	
	protected async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object parameters, CancellationToken cancellationToken)
	{
		try
		{
			await using var connection = ConnectionFactory.CreateConnection();
			await connection.OpenAsync(cancellationToken);
		
			var result = await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);

			await connection.CloseAsync();
			return result;
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Error querying data from db");
			throw;
		}
	}
	
	protected async Task<bool> ExecuteAsync(string sql, object parameters, CancellationToken cancellationToken)
	{
		try
		{
			await using var connection = ConnectionFactory.CreateConnection();
			await connection.OpenAsync(cancellationToken);
		
			var rows = await connection.ExecuteAsync(sql, parameters);
		
			await connection.CloseAsync();
			return rows > 0;
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Error executing command in db");
			throw;
		}
	}
}