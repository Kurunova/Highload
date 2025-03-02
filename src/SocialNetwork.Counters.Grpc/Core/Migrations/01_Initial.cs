using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using SocialNetwork.Counters.Grpc.Core.DataAccess;

namespace SocialNetwork.DataAccess.Migrations;

[Migration(1, "Init")]
public class InitMigration : CustomMigration
{
	private const string SchemaName = "counters_schema";

	protected override void GetUp(IMigrationContext context)
	{
		var sqlStatement = $@"
	        DO $$ 
	        BEGIN
	            -- Проверяем, существует ли схема, если нет – создаем
	            IF NOT EXISTS (SELECT 1 FROM information_schema.schemata WHERE schema_name = '{SchemaName}') THEN
	                CREATE SCHEMA {SchemaName};
	            END IF;

	            -- Создаем таблицу в этой схеме, если она еще не создана
	            IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = '{SchemaName}' AND table_name = 'unread_messages') THEN
	                CREATE TABLE {SchemaName}.unread_messages (
	                    user_id BIGINT PRIMARY KEY,
	                    unread_count INT NOT NULL DEFAULT 0
	                );
	            END IF;
	        END $$;
	        ";
		context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = sqlStatement });
	}

	protected override void GetDown(IMigrationContext context)
	{
		var sqlStatement = $@"
	        DO $$ 
	        BEGIN
	            -- Удаляем таблицу, если она есть
	            IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = '{SchemaName}' AND table_name = 'unread_messages') THEN
	                DROP TABLE {SchemaName}.unread_messages;
	            END IF;
	        END $$;
	        ";
		context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = sqlStatement });
	}
}