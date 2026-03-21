using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class _20260322010117_AddingUserTableMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            CREATE TABLE IF NOT EXISTS users (
                id              SERIAL          PRIMARY KEY,
                name            VARCHAR(255)    NOT NULL,
                email           VARCHAR(255)    NOT NULL UNIQUE,
                password_hash   VARCHAR(512)    NOT NULL,
                password_salt   VARCHAR(512)    NOT NULL,
                created_at      TIMESTAMPTZ     NOT NULL DEFAULT NOW(),
                is_deleted      BOOLEAN         NOT NULL DEFAULT FALSE,
                is_active       BOOLEAN         NOT NULL DEFAULT TRUE
            );
            """;

        await command.ExecuteNonQueryAsync();
    }
}
