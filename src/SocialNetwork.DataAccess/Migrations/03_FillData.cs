using System.Security.Cryptography;
using System.Text;
using Bogus;
using Bogus.DataSets;
using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using SocialNetwork.DataAccess.Services;
using SocialNetwork.Domain.Entities;

namespace SocialNetwork.DataAccess.Migrations;

[Migration(3, "FillData")]
public class FillDataMigration : CustomMigration
{
	private readonly Faker _faker = new("ru");

	protected override void GetUp(IMigrationContext context)
	{
		var sqlBuilder = new StringBuilder();
		int totalRecordsCount = 1000000;
		int insertSize = 100;
		var operationsCount = totalRecordsCount / insertSize;

		for (int operationsCounter = 0; operationsCounter < operationsCount; operationsCounter++)
		{
			var valuesList = new List<string>();
			for (int insertCounter = 0; insertCounter < insertSize; insertCounter++)
			{
				var uniq = operationsCounter * insertSize + insertCounter;
				var genders = Enum.GetValues(typeof(GenderType)).Cast<GenderType>().Where(g => g != GenderType.None).ToArray();
				var gender = _faker.PickRandom(genders);

				var login = $"{_faker.Internet.UserName()}_{uniq}";
				var passwordHash = "kjSTuVknhtcexD3ULD7wRQe9ZKRX+7prvGMJ8QiKXrg=";
				var salt = "mb4B6/dvmrBujt6KzeHa7w==";
				var firstName = _faker.Name.FirstName(gender == GenderType.Male ? Name.Gender.Male : Name.Gender.Female);
				var lastName = _faker.Name.LastName(gender == GenderType.Male ? Name.Gender.Male : Name.Gender.Female);
				var startDate = new DateTime(1955, 1, 1);
				var endDate = DateTime.Today.AddYears(-18);
				var birthdate = _faker.Date.Between(startDate, endDate).ToString("yyyy-MM-dd");

				var city = _faker.Address.City();
				var hobbies = _faker.Lorem.Sentence(wordCount: 5);
			
				valuesList.Add($"('{login}', '{passwordHash}', '{salt}', " +
				               $"'{firstName}', '{lastName}', '{birthdate}', '{gender.ToString()}', " +
				               $"'{city}', '{hobbies}')");
			}

			sqlBuilder.AppendLine("INSERT INTO public.users (login, passwordHash, salt, firstname, lastname, birthdate, gender, city, hobbies) VALUES");
			sqlBuilder.AppendLine(string.Join(",\n", valuesList));
			sqlBuilder.AppendLine(";");

			var sqlStatement = sqlBuilder.ToString();
			context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = sqlStatement });
			sqlBuilder.Clear();
		}
	}

	protected override void GetDown(IMigrationContext context)
	{
		var sqlStatement = @"DELETE FROM public.users;";
		context.Expressions.Add(new ExecuteSqlStatementExpression { SqlStatement = sqlStatement });
	}
}