using FluentMigrator;

namespace SocialNetwork.DataAccess.Migrations;

[Migration(2, "AlterTable")]
public class AlterTableMigration : Migration
{
	public override void Up()
	{
		Execute.Sql(@"
		do $$
		begin
		    ALTER TABLE Users
			RENAME COLUMN SecondName TO LastName;

			ALTER TABLE Users ADD COLUMN Salt_temp TEXT;

			UPDATE Users
			SET Salt_temp = encode(Salt, 'base64');

			ALTER TABLE Users DROP COLUMN Salt;

			ALTER TABLE Users RENAME COLUMN Salt_temp TO Salt;
			
		end $$
		");
	}

	public override void Down()
	{
		Execute.Sql(@"
			ALTER TABLE Users
			RENAME COLUMN LastName TO SecondName;
		");
	}
}