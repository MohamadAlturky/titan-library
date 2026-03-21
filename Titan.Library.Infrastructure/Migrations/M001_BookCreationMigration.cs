using System.Data;
using System.Data.Common;
using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Migrations.Abstractions;

namespace Titan.Library.Infrastructure.Migrations;

public class M001_BookCreationMigration(IDbConnectionFactory dbConnectionFactory)
    : SqlMigration(dbConnectionFactory)
{
    protected override async Task ApplySqlDdl()
    {
        await using var command = Connection.CreateCommand();

        command.CommandText = $"""
            CREATE TABLE IF NOT EXISTS books (
                id UUID PRIMARY KEY,
                title TEXT NOT NULL,
                author TEXT NOT NULL,
                isbn VARCHAR(20) UNIQUE,
                published_date DATE,
                created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
            );

            CREATE INDEX IF NOT EXISTS idx_books_title ON books(title);
            """;

        // Pure ADO.NET Async execution
        await command.ExecuteNonQueryAsync();
    }
}
