using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class _20260322010316_AddingAuthorAndCustomerAndAdminsMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            CREATE TABLE IF NOT EXISTS authors (
                user_id INT PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS customers (
                user_id INT PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS admins (
                user_id INT PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE
            );
            """;

        await command.ExecuteNonQueryAsync();
    }
}
