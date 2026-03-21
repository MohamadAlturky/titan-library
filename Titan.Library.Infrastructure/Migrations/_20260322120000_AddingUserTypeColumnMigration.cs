using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class _20260322120000_AddingUserTypeColumnMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            ALTER TABLE users
                ADD COLUMN IF NOT EXISTS user_type int NOT NULL DEFAULT 1;

            UPDATE users u SET user_type = 2 FROM authors a WHERE a.user_id = u.id;
            UPDATE users u SET user_type = 3  FROM admins  a WHERE a.user_id = u.id;
            """;

        await command.ExecuteNonQueryAsync();
    }
}
