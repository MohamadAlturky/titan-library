using System.Data;
using System.Data.Common;
using Titan.Library.Infrastructure.Connectors;

namespace Titan.Library.Infrastructure.Migrations.Abstractions;

public abstract class SqlMigration(IDbConnectionFactory dbConnectionFactory) : ISqlMigration
{
    protected readonly DbConnection Connection = dbConnectionFactory.CreateDbConnection();

    protected abstract Task ApplySqlDdl();

    public async Task Apply()
    {
        Connection.Open();
        var transaction = Connection.BeginTransaction();
        try
        {
            await ApplySqlDdl();
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public string Key()
    {
        return this.GetType().Name;
    }

    public abstract int Order();
}