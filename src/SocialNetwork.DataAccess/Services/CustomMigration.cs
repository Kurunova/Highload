using FluentMigrator;
using FluentMigrator.Infrastructure;

namespace SocialNetwork.DataAccess.Services;

public abstract class CustomMigration : IMigration
{
    protected abstract void GetUp(IMigrationContext context);
    protected abstract void GetDown(IMigrationContext context);
    
    public void GetUpExpressions(IMigrationContext context) => GetUp(context);

    public void GetDownExpressions(IMigrationContext context) => GetDown(context);

    public object ApplicationContext { get; }
    public string ConnectionString { get; }
}