using System.Data.Common;
using Titan.Library.Domain.Users;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Contexts;
using ADT = Titan.Library.Infrastructure.Configurations.UserTableConfiguration.AdminTable;
using C = Titan.Library.Infrastructure.Configurations.UserTableConfiguration.Columns;
using T = Titan.Library.Infrastructure.Configurations.UserTableConfiguration;

namespace Titan.Library.Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly ISqlDbContext _dbContext;

    public AdminRepository(ISqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Add(Admin entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            WITH inserted AS (
                INSERT INTO {T.Table} ({C.Name}, {C.Email}, {C.PasswordHash}, {C.PasswordSalt})
                VALUES (@Name, @Email, @PasswordHash, @PasswordSalt)
                RETURNING id
            )
            INSERT INTO {ADT.Table} ({ADT.UserId}) SELECT id FROM inserted RETURNING {ADT.UserId};
            """;

        command.AddParameters(
            new
            {
                entity.Name,
                entity.Email,
                entity.PasswordHash,
                entity.PasswordSalt,
            }
        );

        return await command.ExecuteScalarValueAsync<int>();
    }

    public async Task Update(Admin entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            UPDATE {T.Table}
            SET {C.Name} = @Name, {C.Email} = @Email, {C.PasswordHash} = @PasswordHash, {C.PasswordSalt} = @PasswordSalt
            FROM {ADT.Table}
            WHERE {T.Table}.id = {ADT.Table}.{ADT.UserId} AND {T.Table}.id = @Id;
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

    public async Task Delete(Admin entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            UPDATE {T.Table}
            SET {C.IsDeleted} = TRUE
            FROM {ADT.Table}
            WHERE {T.Table}.id = {ADT.Table}.{ADT.UserId} AND {T.Table}.id = @Id;
            """;

        command.AddParameters(new { entity.Id });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task<IEnumerable<Admin>> ToList()
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT u.{C.Id}, u.{C.Name}, u.{C.Email}, u.{C.PasswordHash}, u.{C.PasswordSalt}, u.{C.CreatedAt}, u.{C.IsDeleted}
            FROM {T.Table} u
            INNER JOIN {ADT.Table} a ON u.id = a.{ADT.UserId}
            WHERE u.{C.IsDeleted} = FALSE;
            """;

        return await command.ExecuteListAsync(MapToAdmin);
    }

    public async Task<Admin?> FindById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT u.{C.Id}, u.{C.Name}, u.{C.Email}, u.{C.PasswordHash}, u.{C.PasswordSalt}, u.{C.CreatedAt}, u.{C.IsDeleted}
            FROM {T.Table} u
            INNER JOIN {ADT.Table} a ON u.id = a.{ADT.UserId}
            WHERE u.id = @Id AND u.{C.IsDeleted} = FALSE;
            """;

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToAdmin);
    }

    public async Task<Admin?> FindByEmail(string email)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT u.{C.Id}, u.{C.Name}, u.{C.Email}, u.{C.PasswordHash}, u.{C.PasswordSalt}, u.{C.CreatedAt}, u.{C.IsDeleted}
            FROM {T.Table} u
            INNER JOIN {ADT.Table} a ON u.id = a.{ADT.UserId}
            WHERE u.{C.Email} = @Email AND u.{C.IsDeleted} = FALSE;
            """;

        command.AddParameters(new { Email = email });

        return await command.ExecuteSingleOrDefaultAsync(MapToAdmin);
    }

    private static Admin MapToAdmin(DbDataReader reader)
    {
        var snapshot = new UserSnapshot
        {
            Id           = reader.GetInt32(reader.GetOrdinal(C.Id)),
            Name         = reader.GetString(reader.GetOrdinal(C.Name)),
            Email        = reader.GetString(reader.GetOrdinal(C.Email)),
            PasswordHash = reader.GetString(reader.GetOrdinal(C.PasswordHash)),
            PasswordSalt = reader.GetString(reader.GetOrdinal(C.PasswordSalt)),
            CreatedAt    = reader.GetDateTime(reader.GetOrdinal(C.CreatedAt)),
            IsDeleted    = reader.GetBoolean(reader.GetOrdinal(C.IsDeleted)),
        };
        return Admin.Reconstitute(snapshot);
    }
}
