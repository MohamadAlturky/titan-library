using Microsoft.Extensions.Options;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Services;

public class AdminUserSeeder
{
    private readonly IAdminRepository _adminRepository;
    private readonly AdminSettings _adminSettings;

    public AdminUserSeeder(IAdminRepository adminRepository, IOptions<AdminSettings> adminOptions)
    {
        _adminRepository = adminRepository;
        _adminSettings = adminOptions.Value;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // 1. Check if the admin already exists using the configured email
        var existingAdmin = await _adminRepository.FindByEmail(_adminSettings.Email);

        if (existingAdmin == null)
        {
            // 2. Create the user object using configured values
            var adminUser = Admin.Create(
                _adminSettings.Username,
                _adminSettings.Email,
                _adminSettings.Password
            );

            await _adminRepository.Add(adminUser);
        }
    }
}

public class AdminSettings
{
    public const string SectionName = "AdminSettings";

    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
