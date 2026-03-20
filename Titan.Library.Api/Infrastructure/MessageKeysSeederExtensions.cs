using Titan.Library.Application.Messages.Services;

namespace Titan.Library.Api.Infrastructure;

public static class MessageKeysSeederExtensions
{
    public static async Task UseMessageKeysSeeder(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<ApplicationMessageKeysDataSeeder>();
        await seeder.SeedAsync();
    }
}
