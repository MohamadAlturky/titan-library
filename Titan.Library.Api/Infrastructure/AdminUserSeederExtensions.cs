using Titan.Library.Application.Services;

namespace Titan.Library.Api.Infrastructure;

public static class AdminUserSeederExtensions
{
    public static async Task UseAdminUserSeeder(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<AdminUserSeeder>();
        await seeder.SeedAsync();
    }
}
