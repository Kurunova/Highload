using FluentMigrator;

namespace Ozon.Route256.Practice.OrdersService.Migrations;

[Migration(1, "Init")]
public class InitMigration : Migration
{
	public override void Up()
	{
		Execute.Sql(@"
		do $$
		begin
		    if not exists (select 1 from pg_type where typname = 'gender_type') then        
		        CREATE TYPE public.gender_type AS ENUM ('None', 'Male', 'Female');
		    end if;

			CREATE TABLE IF NOT EXISTS Users (
			    Id BIGSERIAL PRIMARY KEY,
			    Login VARCHAR(255) NOT NULL UNIQUE,
			    PasswordHash TEXT NOT NULL,
			    Salt BYTEA NOT NULL,
			    FirstName VARCHAR(255),
			    SecondName VARCHAR(255),
			    Birthdate DATE,
			    Gender public.gender_type,
			    City VARCHAR(255),
			    Hobbies TEXT
			);
		end $$
		");
	}

	public override void Down()
	{
		Execute.Sql(@"
			DROP TABLE IF EXISTS Users;

			DROP TYPE if exists public.gender_type CASCADE;
		");
	}
}