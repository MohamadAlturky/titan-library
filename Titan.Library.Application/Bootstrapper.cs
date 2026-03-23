using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Titan.Library.Application.Borrows.Strategies;
using Titan.Library.Application.Messages.Caching;
using Titan.Library.Application.Messages.Services;
using Titan.Library.Application.Services;

namespace Titan.Library.Application;

public static class ApplicationBootstrapper
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        services.AddTransient<ApplicationMessageKeysDataSeeder>();
        services.AddTransient<AdminUserSeeder>();
        services.Configure<AdminSettings>(configuration);

        services.AddScoped<IMessageCacheKeyResolver, MessageCacheKeyResolver>();
        services.AddScoped<IMessageCacheValueResolver, MessageCacheValueResolver>();

        // ── Borrow concurrency strategy ────────────────────────────────────────
        // Uncomment exactly ONE of the four lines below to activate a strategy.
        //
        //   Strategy 1 – Atomic UPDATE: check + mark unavailable in a single SQL statement.
        services.AddScoped<IBorrowConcurrencyStrategy, AtomicUpdateBorrowStrategy>();
        //
        //   Strategy 2 – Pessimistic Locking: SELECT … FOR UPDATE blocks concurrent readers.
        // services.AddScoped<IBorrowConcurrencyStrategy, PessimisticLockingBorrowStrategy>();
        //
        //   Strategy 3 – Optimistic Locking: read xmin, update only if xmin unchanged.
        // services.AddScoped<IBorrowConcurrencyStrategy, OptimisticLockingBorrowStrategy>();
        //
        //   Strategy 4 – Serializable isolation: no manual locking, DB retries on conflict.
        // services.AddScoped<IBorrowConcurrencyStrategy, SerializableBorrowStrategy>();
        // ───────────────────────────────────────────────────────────────────────

        return services;
    }
}
