using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Titan.Library.Domain.Books;
using Titan.Library.Domain.Borrows;
using Titan.Library.Domain.Caching;
using Titan.Library.Domain.Users;
using Titan.Library.Infrastructure.Caching;
using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Contexts;
using Titan.Library.Infrastructure.Migrations;
using Titan.Library.Infrastructure.Migrations.Abstractions;
using Titan.Library.Infrastructure.Migrations.Utils;
using Titan.Library.Infrastructure.Repositories;

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

        var redisConnection = configuration.GetConnectionString("RedisConnection")
            ?? throw new InvalidOperationException("RedisConnection connection string is missing.");

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConnection));

        services.AddSingleton<ICacheService, RedisCacheService>();

        services.AddSingleton<IDbConnectionFactory>(_ => new PostgresDbConnectionFactory(
            connectionString
        ));
        services.AddScoped<ISqlDbContext, SqlDbContext>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IBorrowRepository, BorrowRepository>();
        services.AddScoped<IBookTransactionHistoryRepository, BookTransactionHistoryRepository>();
        services.AddScoped<IDbMigrator, SqlDbMigrator>();
        services.RegisterSqlMigrations();

        return services;
    }

    private static void RegisterSqlMigrations(this IServiceCollection services)
    {
        var migrationType = typeof(ISqlMigration);

        var implementations = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => migrationType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var implementation in implementations)
        {
            services.AddTransient(migrationType, implementation);
        }
    }
}
