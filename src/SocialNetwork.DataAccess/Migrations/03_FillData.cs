using System.Security.Cryptography;
using System.Text;
using Bogus;
using Bogus.DataSets;
using FluentMigrator;
using SocialNetwork.Domain.Entities;

namespace Ozon.Route256.Practice.OrdersService.Migrations;

[Migration(3, "FillData")]
public class FillDataMigration : Migration
{
	private readonly Faker _faker = new("ru");
	
	public override void Up()
	{
		for (int i = 0; i < 100000; i++)
		{
			var sqlBuilder = new StringBuilder();

			sqlBuilder.AppendLine("INSERT INTO public.users (login, passwordHash, salt, firstname, lastname, birthdate, gender, city, hobbies) VALUES");
		
			var valuesList = new List<string>();
			for (int j = 0; j < 10; j++)
			{
				var uniq = i * 10 + j;
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

			sqlBuilder.AppendLine(string.Join(",\n", valuesList));
			sqlBuilder.AppendLine(";");
		
			Execute.Sql(sqlBuilder.ToString());
		}
	}

	public override void Down()
	{
		Execute.Sql(@"
		");
	}
}