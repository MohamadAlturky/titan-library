using Titan.Library.Infrastructure.Migrations.Utils;

namespace Titan.Library.Api.Infrastructure;

public static class MigrationExtensions
{
    public static async Task UseSqlMigrations(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<IDbMigrator>>();
        var migrator = services.GetRequiredService<IDbMigrator>();

        try
        {
            logger.LogInformation("Starting database migrations...");

            await migrator.MigrateAsync();

            logger.LogInformation("Database migrations completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogCritical(
                ex,
                "A critical error occurred during database migration. Application startup aborted."
            );
            throw;
        }
    }
}
