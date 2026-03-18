using System.Data;
using System.Data.Common;
using Npgsql;

namespace Titan.Library.Infrastructure.Connectors;

public class PostgresDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public PostgresDbConnectionFactory(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        _connectionString = connectionString;
    }

    public DbConnection CreateDbConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
