namespace Titan.Library.Domain.Users;

public class Admin : User
{
    private Admin()
        : base()
    {
        RestoreUserType(UserType.Admin);
    }

    public UserSnapshot TakeSnapshot() =>
        new()
        {
            Id = Id,
            Name = Name,
            Email = Email,
            PasswordHash = PasswordHash,
            PasswordSalt = PasswordSalt,
            CreatedAt = CreatedAt,
            IsDeleted = IsDeleted,
            IsActive = IsActive,
            UserType = UserType,
        };

    public static Admin Create(string name, string email, string password)
    {
        var a = new Admin
        {
            Name = name,
            Email = email,
            CreatedAt = DateTime.UtcNow,
        };
        a.SetPassword(password);
        return a;
    }

    public static Admin Reconstitute(UserSnapshot snapshot)
    {
        var a = new Admin
        {
            Id = snapshot.Id,
            Name = snapshot.Name,
            Email = snapshot.Email,
            CreatedAt = snapshot.CreatedAt,
        };
        a.RestorePassword(snapshot.PasswordHash, snapshot.PasswordSalt);
        a.RestoreIsDeleted(snapshot.IsDeleted);
        a.RestoreIsActive(snapshot.IsActive);
        a.RestoreUserType(snapshot.UserType);
        return a;
    }
}
