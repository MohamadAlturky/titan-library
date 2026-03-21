namespace Titan.Library.Infrastructure.Migrations.Abstractions;

public interface ISqlMigration
{
    string Key();
    Task Apply();
}
