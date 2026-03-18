namespace Titan.Library.Infrastructure.Migrations.Utils;

public interface IDbMigrator
{
    Task MigrateAsync();
}