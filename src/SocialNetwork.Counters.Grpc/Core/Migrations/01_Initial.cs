using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using SocialNetwork.Counters.Grpc.Core.DataAccess;

namespace SocialNetwork.DataAccess.Migrations;

[Migration(1, "Init")]
public class InitMigration : CustomMigration
{
	protected override string GetUpSql(IServiceProvider services)
	{
		return $@"
	        DO $$ 
	        BEGIN
	            IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'unread_messages') THEN
	                CREATE TABLE unread_messages (
	                    user_id BIGINT PRIMARY KEY,
	                    unread_count INT NOT NULL DEFAULT 0
	                );
	            END IF;
	        END $$;
	    ";
	}

	protected override string GetDownSql(IServiceProvider services)
	{
		return $@"
	        DO $$ 
	        BEGIN
	            -- Удаляем таблицу, если она есть
	            IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'unread_messages') THEN
	                DROP TABLE unread_messages;
	            END IF;
	        END $$;
	    ";
	}
}