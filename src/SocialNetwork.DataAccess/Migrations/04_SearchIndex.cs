using FluentMigrator;

namespace SocialNetwork.DataAccess.Migrations;

[Migration(4, "SearchIndex")]
public class SearchIndexMigration : Migration
{
	public override void Up()
	{
		Execute.Sql(@"
		do $$
		begin
			CREATE EXTENSION IF NOT EXISTS pg_trgm;

			CREATE INDEX IF NOT EXISTS idx_users_firstname_trgm ON Users USING GIN (FirstName gin_trgm_ops);
			CREATE INDEX IF NOT EXISTS idx_users_lastname_trgm ON Users USING GIN (LastName gin_trgm_ops);
		end $$
		");
	}

	public override void Down()
	{
		Execute.Sql(@"
		do $$
		begin
			DROP INDEX IF EXISTS idx_users_firstname_trgm;
			DROP INDEX IF EXISTS idx_users_lastname_trgm;

			IF NOT EXISTS (SELECT 1 FROM pg_indexes WHERE indexdef LIKE '%gin_trgm_ops%') THEN
				DROP EXTENSION IF EXISTS pg_trgm;
			END IF;
		end $$
		");
	}
}