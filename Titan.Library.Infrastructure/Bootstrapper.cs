using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using StackExchange.Redis;
using Titan.Library.Common.Auth;
using Titan.Library.Common.Caching;
using Titan.Library.Common.Storage;
using Titan.Library.Domain.Books;
using Titan.Library.Domain.Borrows;
using Titan.Library.Domain.Feedbacks;
using Titan.Library.Domain.Messages;
using Titan.Library.Domain.Users;
using Titan.Library.Infrastructure.Auth;
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

        var redisConnection =
            configuration.GetConnectionString("RedisConnection")
            ?? throw new InvalidOperationException("RedisConnection connection string is missing.");

        services.AddJwtAuth(configuration);

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConnection)
        );

        services.AddSingleton<ICacheService, RedisCacheService>();

        var pooling =
            configuration
                .GetSection(PostgresPoolingOptions.SectionName)
                .Get<PostgresPoolingOptions>()
            ?? new PostgresPoolingOptions();

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Pooling = true,
            MinPoolSize = pooling.MinPoolSize,
            MaxPoolSize = pooling.MaxPoolSize,
            ConnectionIdleLifetime = pooling.ConnectionIdleLifetimeSeconds,
            Timeout = pooling.ConnectionTimeoutSeconds,
        };

        var dataSource = new NpgsqlDataSourceBuilder(
            connectionStringBuilder.ConnectionString
        ).Build();
        services.AddSingleton(dataSource);
        services.AddSingleton<IDbConnectionFactory>(_ => new PostgresDbConnectionFactory(
            dataSource
        ));
        services.AddScoped<ISqlDbContext, SqlDbContext>();
        services.AddScoped<IAsyncUnitOfWork>(sp => sp.GetRequiredService<ISqlDbContext>());
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IBorrowRepository, BorrowRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IDbMigrator, SqlDbMigrator>();
        services.RegisterSqlMigrations();

        return services;
    }

    private static void AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions =
            configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("JwtSettings configuration section is missing.");

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<IJwtGenerator, JwtGenerator>();

        var key = Encoding.UTF8.GetBytes(jwtOptions.Secret);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                };
            });

        services.AddAuthorization();
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
