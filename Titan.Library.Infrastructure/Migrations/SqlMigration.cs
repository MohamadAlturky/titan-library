using System.Data;
using Titan.Library.Infrastructure.Connectors;

namespace Titan.Library.Infrastructure.Migrations;

public abstract class SqlMigration : ISqlMigration
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    protected readonly IDbConnection connection;

    protected SqlMigration(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
        connection = _dbConnectionFactory.CreateDbConnection();
    }

    public abstract Task ApplySqlDdl();

    public async Task Apply()
    {
        connection.Open();
        IDbTransaction transaction = connection.BeginTransaction();
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
}
