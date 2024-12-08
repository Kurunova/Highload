using FluentMigrator;

namespace SocialNetwork.DataAccess.Migrations;

[Migration(5, "AddPostsAndUserFriends")]
public class AddPostsAndUserFriendsMigration : Migration
{
	public override void Up()
	{
		Execute.Sql(@"
            CREATE TABLE IF NOT EXISTS Posts (
                Id BIGSERIAL PRIMARY KEY,
                AuthorUserId BIGINT NOT NULL,
                Text TEXT NOT NULL,
                CreatedAt TIMESTAMP DEFAULT NOW(),
                FOREIGN KEY (AuthorUserId) REFERENCES Users(Id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS UserFriends (
                UserId BIGINT NOT NULL,
                FriendId BIGINT NOT NULL,
                PRIMARY KEY (UserId, FriendId),
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
                FOREIGN KEY (FriendId) REFERENCES Users(Id) ON DELETE CASCADE
            );
        ");
	}

	public override void Down()
	{
		Execute.Sql(@"
            DROP TABLE IF EXISTS UserFriends;
            DROP TABLE IF EXISTS Posts;
        ");
	}
}