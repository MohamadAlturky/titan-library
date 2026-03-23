using Microsoft.AspNetCore.Identity;
using Titan.Library.Domain.Users; // Assuming you use Identity for hashing

namespace Titan.Library.Application.Services;

public class AdminUserSeeder
{
    private readonly IAdminRepository _adminRepository;

    private const string AdminEmail = "titan@library.com";
    private const string AdminPassword = "ISDhusd98sdhsd98otasdg";

    public AdminUserSeeder(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // 1. Check if the admin already exists
        var existingAdmin = await _adminRepository.FindByEmail(AdminEmail);

        if (existingAdmin == null)
        {
            // 2. Create the user object
            var adminUser = new Admin
            {
                Email = AdminEmail,
                Name = AdminEmail,
                CreatedAt = DateTime.UtcNow,
            };
            adminUser.SetPassword(AdminPassword);

            await _adminRepository.Add(adminUser);
        }
    }
}
