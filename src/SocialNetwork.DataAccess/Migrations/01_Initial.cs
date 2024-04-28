using FluentMigrator;

namespace Ozon.Route256.Practice.OrdersService.Migrations;

[Migration(1, "Init")]
public class InitMigration : Migration
{
	public override void Up()
	{
		Execute.Sql(@"
			CREATE TABLE IF NOT EXISTS Users (
			    Id BIGSERIAL PRIMARY KEY,
			    FirstName VARCHAR(100) NOT NULL,
			    SecondName VARCHAR(100) NOT NULL,
			    Birthdate DATE NOT NULL,
			    Biography TEXT,
			    City VARCHAR(100) NOT NULL
			);
		");
	}

	public override void Down()
	{
		Execute.Sql(@"
			DROP TABLE IF EXISTS Users;
		");
	}
}