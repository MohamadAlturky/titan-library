using System.Data.Common;
using Titan.Library.Domain.Users;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Contexts;
using AT = Titan.Library.Infrastructure.Configurations.UserTableConfiguration.AuthorTable;
using C = Titan.Library.Infrastructure.Configurations.UserTableConfiguration.Columns;
using T = Titan.Library.Infrastructure.Configurations.UserTableConfiguration;

namespace Titan.Library.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly ISqlDbContext _dbContext;

    public AuthorRepository(ISqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Add(Author entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            WITH inserted AS (
                INSERT INTO {T.Table} ({C.Name}, {C.Email}, {C.PasswordHash}, {C.PasswordSalt}, {C.UserType})
                VALUES (@Name, @Email, @PasswordHash, @PasswordSalt, @UserType)
                RETURNING id
            )
            INSERT INTO {AT.Table} ({AT.UserId}) SELECT id FROM inserted RETURNING {AT.UserId};
            """;

        command.AddParameters(
            new
            {
                entity.Name,
                entity.Email,
                entity.PasswordHash,
                entity.PasswordSalt,
                UserType = ((int)entity.UserType),
            }
        );

        return await command.ExecuteScalarValueAsync<int>();
    }

    public async Task Update(Author entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            UPDATE {T.Table}
            SET {C.Name} = @Name, {C.Email} = @Email, {C.PasswordHash} = @PasswordHash, {C.PasswordSalt} = @PasswordSalt
            FROM {AT.Table}
            WHERE {T.Table}.id = {AT.Table}.{AT.UserId} AND {T.Table}.id = @Id;
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

    public async Task Delete(Author entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            UPDATE {T.Table}
            SET {C.IsDeleted} = TRUE
            FROM {AT.Table}
            WHERE {T.Table}.id = {AT.Table}.{AT.UserId} AND {T.Table}.id = @Id;
            """;

        command.AddParameters(new { entity.Id });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task<IEnumerable<Author>> ToList()
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT u.{C.Id}, u.{C.Name}, u.{C.Email}, u.{C.PasswordHash}, u.{C.PasswordSalt}, u.{C.CreatedAt}, u.{C.IsDeleted}, u.{C.IsActive}, u.{C.UserType}
            FROM {T.Table} u
            INNER JOIN {AT.Table} a ON u.id = a.{AT.UserId}
            WHERE u.{C.IsDeleted} = FALSE;
            """;

        return await command.ExecuteListAsync(MapToAuthor);
    }

    public async Task<Author?> FindById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT u.{C.Id}, u.{C.Name}, u.{C.Email}, u.{C.PasswordHash}, u.{C.PasswordSalt}, u.{C.CreatedAt}, u.{C.IsDeleted}, u.{C.IsActive}, u.{C.UserType}
            FROM {T.Table} u
            INNER JOIN {AT.Table} a ON u.id = a.{AT.UserId}
            WHERE u.id = @Id AND u.{C.IsDeleted} = FALSE;
            """;

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToAuthor);
    }

    public async Task<Author?> FindByEmail(string email)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT u.{C.Id}, u.{C.Name}, u.{C.Email}, u.{C.PasswordHash}, u.{C.PasswordSalt}, u.{C.CreatedAt}, u.{C.IsDeleted}, u.{C.IsActive}, u.{C.UserType}
            FROM {T.Table} u
            INNER JOIN {AT.Table} a ON u.id = a.{AT.UserId}
            WHERE u.{C.Email} = @Email AND u.{C.IsDeleted} = FALSE;
            """;

        command.AddParameters(new { Email = email });

        return await command.ExecuteSingleOrDefaultAsync(MapToAuthor);
    }

    private static Author MapToAuthor(DbDataReader reader)
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
            UserType = Enum.Parse<UserType>(reader.GetString(reader.GetOrdinal(C.UserType)), true),
        };
        return Author.Reconstitute(snapshot);
    }
}
