using FluentMigrator;

namespace SocialNetwork.DataAccess.Migrations;

[Migration(1, "Init")]
public class InitMigration : Migration
{
	public override void Up()
	{
		Execute.Sql(@"
        DO $$ 
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'unread_messages') THEN
                CREATE TABLE unread_messages (
                    user_id BIGINT PRIMARY KEY,
                    unread_count INT NOT NULL DEFAULT 0
                );
            END IF;
        END $$;
        ");
	}

	public override void Down()
	{
		Execute.Sql(@"
            DROP TABLE IF EXISTS unread_messages;
        ");
	}
}