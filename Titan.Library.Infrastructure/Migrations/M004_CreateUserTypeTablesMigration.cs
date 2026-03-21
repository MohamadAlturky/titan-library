using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;
using ADT = Titan.Library.Infrastructure.Configurations.UserTableConfiguration.AdminTable;
using AT = Titan.Library.Infrastructure.Configurations.UserTableConfiguration.AuthorTable;
using CT = Titan.Library.Infrastructure.Configurations.UserTableConfiguration.CustomerTable;
using T = Titan.Library.Infrastructure.Configurations.UserTableConfiguration;

namespace Titan.Library.Infrastructure.Migrations;

public class M004_CreateUserTypeTablesMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            CREATE TABLE IF NOT EXISTS {AT.Table} (
                {AT.UserId} INT PRIMARY KEY REFERENCES {T.Table}(id) ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS {CT.Table} (
                {CT.UserId} INT PRIMARY KEY REFERENCES {T.Table}(id) ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS {ADT.Table} (
                {ADT.UserId} INT PRIMARY KEY REFERENCES {T.Table}(id) ON DELETE CASCADE
            );
            """;

        await command.ExecuteNonQueryAsync();
    }
}
