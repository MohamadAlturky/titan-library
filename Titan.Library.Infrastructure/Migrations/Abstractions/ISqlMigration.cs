namespace Titan.Library.Infrastructure.Migrations.Abstractions;

public interface ISqlMigration
{
    int Order();
    string Key();
    Task Apply();
}
