using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;
using C = Titan.Library.Infrastructure.Configurations.UserTableConfiguration.Columns;
using T = Titan.Library.Infrastructure.Configurations.UserTableConfiguration;

namespace Titan.Library.Infrastructure.Migrations;

public class M003_CreateUsersTableMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            CREATE TABLE IF NOT EXISTS {T.Table} (
                {C.Id}           SERIAL PRIMARY KEY,
                {C.Name}         TEXT NOT NULL,
                {C.Email}        TEXT NOT NULL UNIQUE,
                {C.PasswordHash} TEXT NOT NULL,
                {C.PasswordSalt} TEXT NOT NULL,
                {C.CreatedAt}    TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                {C.IsDeleted}    BOOLEAN NOT NULL DEFAULT FALSE,
                {C.IsActive}     BOOLEAN NOT NULL DEFAULT TRUE
            );
            CREATE INDEX IF NOT EXISTS idx_users_email ON {T.Table}({C.Email});
            """;

        await command.ExecuteNonQueryAsync();
    }
}
