using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using Microsoft.Extensions.Options;
using SocialNetwork.Counters.Grpc.DataAccess;

namespace SocialNetwork.Counters.Grpc.Core.DataAccess;

public abstract class CustomMigration : IMigration
{
	public void GetUpExpressions(IMigrationContext context)
	{
		var sqlStatement = GetUpSql(context.ServiceProvider);
		
		if(string.IsNullOrWhiteSpace(sqlStatement))
			return;
		
		var currentSchema = context.ServiceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value.Schema;
		
		Console.WriteLine($"Current schema: {currentSchema}");
		if (string.IsNullOrWhiteSpace(currentSchema))
			throw new Exception("Database schema is not set!");
		
		if (!context.QuerySchema.SchemaExists(currentSchema))
		{
			context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = $"create schema {currentSchema};" });
		}

		context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = $"SET search_path TO {currentSchema};" });
		context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = sqlStatement });
	}

	public void GetDownExpressions(IMigrationContext context)
	{
		var sqlStatement = GetDownSql(context.ServiceProvider);

		if(string.IsNullOrWhiteSpace(sqlStatement))
			return;
		
		var currentSchema = context.ServiceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value.Schema;

		context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = $"SET search_path TO {currentSchema};" });
		context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = sqlStatement });
	}
	
	protected abstract string GetUpSql(IServiceProvider services);
	protected abstract string GetDownSql(IServiceProvider services);
    
	public object ApplicationContext { get; }
	public string ConnectionString { get; }
}