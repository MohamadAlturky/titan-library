using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class _20260322010338_AddingIndexesForUsersTablesMigration(
    IDbConnectionFactory dbConnectionFactory
) : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            CREATE UNIQUE INDEX IF NOT EXISTS idx_users_email ON users(email) WHERE is_deleted = false;
            CREATE INDEX IF NOT EXISTS idx_users_is_deleted ON users(is_deleted);
            CREATE INDEX IF NOT EXISTS idx_users_is_active  ON users(is_active);
            """;

        await command.ExecuteNonQueryAsync();
    }
}
