using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Titan.Library.Application.Messages.Caching;
using Titan.Library.Application.Messages.Services;

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

        services.AddScoped<IMessageCacheKeyResolver, MessageCacheKeyResolver>();
        services.AddScoped<IMessageCacheValueResolver, MessageCacheValueResolver>();
        return services;
    }
}
