using System.Security.Cryptography;
using Titan.Library.Common.Abstractions;

namespace Titan.Library.Domain.Users;

public class User : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string PasswordSalt { get; private set; } = string.Empty;

    public void SetPassword(string plainPassword)
    {
        var salt = GenerateSalt();
        PasswordSalt = Convert.ToBase64String(salt);
        PasswordHash = ComputeHash(plainPassword, salt);
    }

    public bool VerifyPassword(string plainPassword)
    {
        var salt = Convert.FromBase64String(PasswordSalt);
        var hash = ComputeHash(plainPassword, salt);
        return hash == PasswordHash;
    }

    private static byte[] GenerateSalt()
    {
        return RandomNumberGenerator.GetBytes(32);
    }

    public bool IsDeleted { get; private set; }

    public void Delete() => IsDeleted = true;

    protected void RestoreIsDeleted(bool isDeleted) => IsDeleted = isDeleted;

    protected void RestorePassword(string passwordHash, string passwordSalt)
    {
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    private static string ComputeHash(string password, byte[] salt)
    {
        var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
        var combined = new byte[passwordBytes.Length + salt.Length];
        Buffer.BlockCopy(passwordBytes, 0, combined, 0, passwordBytes.Length);
        Buffer.BlockCopy(salt, 0, combined, passwordBytes.Length, salt.Length);

        var hash = SHA256.HashData(combined);
        return Convert.ToBase64String(hash);
    }
}
