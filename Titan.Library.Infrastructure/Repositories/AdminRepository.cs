using System.Data.Common;
using Npgsql;
using NpgsqlTypes;
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
                INSERT INTO {T.Table} ({C.Name}, {C.Email}, {C.PasswordHash}, {C.PasswordSalt}, {C.UserType})
                VALUES (@Name, @Email, @PasswordHash, @PasswordSalt, @UserType)
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
                UserType = ((int)entity.UserType),
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
            SELECT u.{C.Id}, u.{C.Name}, u.{C.Email}, u.{C.PasswordHash}, u.{C.PasswordSalt}, u.{C.CreatedAt}, u.{C.IsDeleted}, u.{C.IsActive}
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
            SELECT u.{C.Id}, u.{C.Name}, u.{C.Email}, u.{C.PasswordHash}, u.{C.PasswordSalt}, u.{C.CreatedAt}, u.{C.IsDeleted}, u.{C.IsActive}
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
            SELECT u.{C.Id}, u.{C.Name}, u.{C.Email}, u.{C.PasswordHash}, u.{C.PasswordSalt}, u.{C.CreatedAt}, u.{C.IsDeleted}, u.{C.IsActive}
            FROM {T.Table} u
            INNER JOIN {ADT.Table} a ON u.id = a.{ADT.UserId}
            WHERE u.{C.Email} = @Email AND u.{C.IsDeleted} = FALSE;
            """;

        command.AddParameters(new { Email = email });

        return await command.ExecuteSingleOrDefaultAsync(MapToAdmin);
    }

    public async Task<(List<User> items, int total)> GetUsersPaginated(
        string? search,
        int? userType,
        string orderBy,
        bool ascending,
        int page,
        int pageSize
    )
    {
        await using var command = await _dbContext.CreateCommandAsync();

        var direction = ascending ? "ASC" : "DESC";
        var allowedSortColumns = new HashSet<string> { "id", "name", "email", "created_at", "is_active" };
        var sortColumn = allowedSortColumns.Contains(orderBy) ? orderBy : "id";
        var searchParam = search is not null ? $"%{search}%" : null;
        var offset = (page - 1) * pageSize;

        command.CommandText = $"""
            SELECT u.{C.Id}, u.{C.Name}, u.{C.Email}, u.{C.PasswordHash}, u.{C.PasswordSalt},
                   u.{C.CreatedAt}, u.{C.IsDeleted}, u.{C.IsActive}, u.{C.UserType},
                   COUNT(*) OVER() AS total_count
            FROM {T.Table} u
            WHERE u.{C.IsDeleted} = FALSE
                AND u.{C.UserType} IN (1, 3)
                AND (@UserType::integer IS NULL OR u.{C.UserType} = @UserType)
                AND (@Search IS NULL OR u.{C.Name} ILIKE @Search OR u.{C.Email} ILIKE @Search)
            ORDER BY {sortColumn} {direction}
            LIMIT @PageSize OFFSET @Offset;
            """;

        command.AddParameters(new { PageSize = pageSize, Offset = offset });
        command.Parameters.Add(
            new NpgsqlParameter("UserType", NpgsqlDbType.Integer)
            {
                Value = userType.HasValue ? (object)userType.Value : DBNull.Value,
            }
        );
        command.Parameters.Add(
            new NpgsqlParameter("Search", NpgsqlDbType.Text)
            {
                Value = searchParam ?? (object)DBNull.Value,
            }
        );

        var items = new List<User>();
        var total = 0;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (total == 0)
                total = (int)reader.GetInt64(reader.GetOrdinal("total_count"));
            items.Add(MapToUser(reader));
        }

        return (items, total);
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
            _ => throw new InvalidOperationException($"Unexpected user type: {snapshot.UserType}"),
        };
    }

    private static Admin MapToAdmin(DbDataReader reader)
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
        };
        return Admin.Reconstitute(snapshot);
    }
}
