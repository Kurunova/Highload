using System.Text;
using Bogus;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using SocialNetwork.DataAccess.Services;

namespace SocialNetwork.DataAccess.Migrations;

[Migration(7, "DialogMessagesMigration")]
public class DialogMessagesMigration : Migration
{
	public override void Up()
	{
		Execute.Sql(@"
            CREATE TABLE dialog_messages (
                id BIGSERIAL PRIMARY KEY,
                from_user_id BIGINT NOT NULL,
                to_user_id BIGINT NOT NULL,
                text TEXT NOT NULL,
                sent_at TIMESTAMP WITHOUT TIME ZONE NOT NULL
            )
        ");
	}

	public override void Down()
	{
		Execute.Sql("DROP TABLE IF EXISTS dialog_messages");
	}
}