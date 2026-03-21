using System.Data;
using System.Data.Common;
using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations.Utils;

public class SqlDbMigrator(
    IDbConnectionFactory connectionFactory,
    IEnumerable<ISqlMigration> migrations
) : IDbMigrator
{
    private const string MigrationTableName = "__SQL_MIGRATIONS";

    public async Task MigrateAsync()
    {
        // Assuming your factory returns a DbConnection (which supports Async methods)
        await using var connection =
            connectionFactory.CreateDbConnection()
            ?? throw new InvalidOperationException(
                "Connection must be a DbConnection to support async operations."
            );

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        // 1. Ensure the tracking table exists
        await EnsureMigrationTableExists(connection);

        // 2. Get already applied migration keys
        var appliedKeys = await GetAppliedMigrationKeys(connection);

        // 3. Filter and Sort pending migrations
        var pendingMigrations = migrations
            .Where(m => !appliedKeys.Contains(m.Key()))
            .OrderBy(m => m.Order());

        foreach (var migration in pendingMigrations)
        {
            // We use a manual transaction for the "Record Keeping" step
            // Note: If ISqlMigration.Apply() manages its own connection,
            // you might need to pass this connection/transaction to it.
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                // 4. Apply the migration
                await migration.Apply();

                // 5. Record the execution
                await RecordMigrationSuccess(connection, transaction, migration.Key());

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    private async Task EnsureMigrationTableExists(DbConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"""
            CREATE TABLE IF NOT EXISTS {MigrationTableName} (
                migration_key TEXT PRIMARY KEY,
                execution_date TIMESTAMP WITH TIME ZONE NOT NULL
            );
            """;
        await command.ExecuteNonQueryAsync();
    }

    private async Task<HashSet<string>> GetAppliedMigrationKeys(DbConnection connection)
    {
        var keys = new HashSet<string>();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT migration_key FROM {MigrationTableName}";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            keys.Add(reader.GetString(0));
        }

        return keys;
    }

    private async Task RecordMigrationSuccess(
        DbConnection connection,
        DbTransaction transaction,
        string key
    )
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            $"INSERT INTO {MigrationTableName} (migration_key, execution_date) VALUES (@key, @date)";

        var keyParam = command.CreateParameter();
        keyParam.ParameterName = "@key";
        keyParam.Value = key;
        command.Parameters.Add(keyParam);

        var dateParam = command.CreateParameter();
        dateParam.ParameterName = "@date";
        dateParam.Value = DateTime.UtcNow;
        command.Parameters.Add(dateParam);

        await command.ExecuteNonQueryAsync();
    }
}
