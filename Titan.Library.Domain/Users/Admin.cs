namespace Titan.Library.Domain.Users;

public class Admin : User
{
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
        };

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
        return a;
    }
}
