using System.Data;
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

    public IDbConnection CreateDbConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
