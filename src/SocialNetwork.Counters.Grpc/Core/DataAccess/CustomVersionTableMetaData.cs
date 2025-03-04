using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Options;
using SocialNetwork.Counters.Grpc.DataAccess;

namespace SocialNetwork.Counters.Grpc.Core.DataAccess;

public class CustomVersionTableMetaData : IVersionTableMetaData
{
	private readonly string _schema;

	public CustomVersionTableMetaData(IOptions<DatabaseSettings> databaseSettings)
	{
		_schema = databaseSettings?.Value?.Schema;
	}

	public object ApplicationContext { get; set; } = null;
	public bool OwnsSchema => true;
	public string SchemaName => _schema;
	public string TableName => "version_info";
	public string ColumnName => "version";
	public string DescriptionColumnName => "description";
	public string UniqueIndexName => "version_unique_idx";
	public string AppliedOnColumnName => "applied_on";
}