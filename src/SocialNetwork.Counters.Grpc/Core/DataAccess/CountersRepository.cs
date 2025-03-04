using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using SocialNetwork.Counters.Grpc.Domain;

namespace SocialNetwork.Counters.Grpc.DataAccess;

public class CountersRepository : ICountersRepository
{
	private readonly ILogger<CountersRepository> _logger;
    private readonly string _connectionString;
    private readonly string _schema;

    public CountersRepository(ILogger<CountersRepository> logger, IOptions<DatabaseSettings> databaseSettings)
    {
        _logger = logger;
        _connectionString = databaseSettings.Value.ConnectionString;
        _schema = databaseSettings.Value.Schema;
        _connectionString = $"{_connectionString.TrimEnd(';')};SearchPath={_schema}";
    }

    private NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);

    /// <summary>
    /// Получает количество непрочитанных сообщений у пользователя
    /// </summary>
    public async Task<int> GetUnreadMessagesCountAsync(long userId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT unread_count FROM unread_messages WHERE user_id = @UserId";

        await using var connection = CreateConnection();
        var count = await connection.QuerySingleOrDefaultAsync<int?>(sql, new { UserId = userId });

        return count ?? 0;
    }

    /// <summary>
    /// Увеличивает счетчик непрочитанных сообщений для пользователя
    /// </summary>
    public async Task IncrementUnreadMessagesCountAsync(long userId, int count, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO unread_messages (user_id, unread_count) 
            VALUES (@UserId, @Count) 
            ON CONFLICT (user_id) 
            DO UPDATE SET unread_count = unread_messages.unread_count + @Count";

        await using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, new { UserId = userId, Count = count });

        _logger.LogInformation($"Unread messages for User {userId} incremented.");
    }

    /// <summary>
    /// Уменьшает счетчик непрочитанных сообщений для пользователя (не уходит в отрицательные значения)
    /// </summary>
    public async Task DecrementUnreadMessagesCountAsync(long userId, int count, CancellationToken cancellationToken)
    {
        const string sql = @"
            UPDATE unread_messages 
            SET unread_count = unread_count - @Count 
            WHERE user_id = @UserId AND unread_count > 0";

        await using var connection = CreateConnection();
        using var command = connection.CreateCommand();
        var affectedRows = await connection.ExecuteAsync(sql, new { UserId = userId, Count = count });

        if (affectedRows > 0)
            _logger.LogInformation($"Unread messages for User {userId} decremented.");
    }
}