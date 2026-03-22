using System.Data;
using System.Data.Common;
using Titan.Library.Domain.Users;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Contexts;
using C = Titan.Library.Infrastructure.Configurations.UserTableConfiguration.Columns;
using T = Titan.Library.Infrastructure.Configurations.UserTableConfiguration;

namespace Titan.Library.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ISqlDbContext _dbContext;

    public UserRepository(ISqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Add(User entity)
    {
        throw new NotSupportedException("Use a specific repository to add users.");
    }

    public async Task Update(User entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            UPDATE {T.Table}
            SET {C.Name} = @Name, {C.Email} = @Email, {C.PasswordHash} = @PasswordHash, {C.PasswordSalt} = @PasswordSalt
            WHERE id = @Id AND {C.IsDeleted} = FALSE;
            """;

        command.AddParameters(
            new
            {
                entity.Id,
                entity.Name,
                entity.Email,
                entity.PasswordHash,
                entity.PasswordSalt,
            }
        );

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task Delete(User entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            UPDATE {T.Table}
            SET {C.IsDeleted} = TRUE
            WHERE id = @Id;
            """;

        command.AddParameters(new { entity.Id });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task<IEnumerable<User>> ToList()
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT {C.Id}, {C.Name}, {C.Email}, {C.PasswordHash}, {C.PasswordSalt}, {C.CreatedAt}, {C.IsDeleted}, {C.IsActive}, {C.UserType}
            FROM {T.Table}
            WHERE {C.IsDeleted} = FALSE;
            """;

        return await command.ExecuteListAsync(MapToUser);
    }

    public async Task<User?> FindById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT {C.Id}, {C.Name}, {C.Email}, {C.PasswordHash}, {C.PasswordSalt}, {C.CreatedAt}, {C.IsDeleted}, {C.IsActive}, {C.UserType}
            FROM {T.Table}
            WHERE id = @Id AND {C.IsDeleted} = FALSE;
            """;

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToUser);
    }

    public async Task<User?> FindByEmail(string email)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT {C.Id}, {C.Name}, {C.Email}, {C.PasswordHash}, {C.PasswordSalt}, {C.CreatedAt}, {C.IsDeleted}, {C.IsActive}, {C.UserType}
            FROM {T.Table}
            WHERE {C.Email} = @Email AND {C.IsDeleted} = FALSE;
            """;

        command.AddParameters(new { Email = email });

        return await command.ExecuteSingleOrDefaultAsync(MapToUser);
    }

    private static User MapToUser(DbDataReader reader)
    {
        var snapshot = new UserSnapshot
        {
            Id = reader.GetInt32(reader.GetOrdinal(C.Id)),
            Name = reader.GetString(reader.GetOrdinal(C.Name)),
            Email = reader.GetString(reader.GetOrdinal(C.Email)),
            PasswordHash = reader.GetString(reader.GetOrdinal(C.PasswordHash)),
            PasswordSalt = reader.GetString(reader.GetOrdinal(C.PasswordSalt)),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal(C.CreatedAt)),
            IsDeleted = reader.GetBoolean(reader.GetOrdinal(C.IsDeleted)),
            IsActive = reader.GetBoolean(reader.GetOrdinal(C.IsActive)),
            UserType = (UserType)reader.GetInt32(reader.GetOrdinal(C.UserType)),
        };

        return snapshot.UserType switch
        {
            UserType.Customer => Customer.Reconstitute(snapshot),
            UserType.Author => Author.Reconstitute(snapshot),
            UserType.Admin => Admin.Reconstitute(snapshot),
            _ => throw new ArgumentOutOfRangeException(
                nameof(snapshot.UserType),
                $"Unexpected value: {snapshot.UserType}"
            ),
        };
    }
}
