using System.Text;
using Bogus;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using SocialNetwork.DataAccess.Services;

namespace SocialNetwork.DataAccess.Migrations;

[Migration(6, "FillPostsMigration")]
public class FillPostsMigration : CustomMigration
{
	private readonly Faker _faker = new("ru");

	protected override void GetUp(IMigrationContext context)
	{
		var sqlBuilder = new StringBuilder();

		int totalRecordsCount = 1000000; // Общее количество записей
		int insertSize = 1000;          // Размер одной вставки (batch)
		var operationsCount = totalRecordsCount / insertSize;

		for (int operationsCounter = 0; operationsCounter < operationsCount; operationsCounter++)
		{
			var valuesList = new List<string>();

			for (int insertCounter = 0; insertCounter < insertSize; insertCounter++)
			{
				var uniq = operationsCounter * insertSize + insertCounter;
				var authorId = _faker.Random.Long(1, 1000000); // Случайный ID автора (из существующих пользователей)
				var text = _faker.Lorem.Sentence(wordCount: 20); // Случайный текст поста
				var createdAt = _faker.Date.Past(1).ToString("yyyy-MM-dd HH:mm:ss"); // Дата поста за последний год

				valuesList.Add($"({authorId}, '{text.Replace("'", "''")}', '{createdAt}')");
			}

			sqlBuilder.AppendLine("INSERT INTO public.posts (authoruserid, text, createdat) VALUES");
			sqlBuilder.AppendLine(string.Join(",\n", valuesList));
			sqlBuilder.AppendLine(";");

			var sqlStatement = sqlBuilder.ToString();
			context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = sqlStatement });
			sqlBuilder.Clear();
		}
	}

	protected override void GetDown(IMigrationContext context)
	{
		var sqlStatement = @"DELETE FROM public.posts;";
		context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = sqlStatement });
	}
}