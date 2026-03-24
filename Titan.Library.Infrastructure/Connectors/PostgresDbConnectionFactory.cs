using System.Data.Common;
using Npgsql;

namespace Titan.Library.Infrastructure.Connectors;

public class PostgresDbConnectionFactory : IDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresDbConnectionFactory(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public DbConnection CreateDbConnection()
    {
        return _dataSource.CreateConnection();
    }
}
