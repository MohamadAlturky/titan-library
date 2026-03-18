using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Titan.Library.Infrastructure.Connectors;

namespace Titan.Library.Infrastructure;

public static class InfrastructureBootstrapper
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "DefaultConnection"
    )
    {
        var connectionString = configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{connectionStringName}' was not found or is empty. Please check your configuration."
            );
        }

        services.AddSingleton<IDbConnectionFactory>(_ => new PostgresDbConnectionFactory(
            connectionString
        ));

        return services;
    }
}
