using System.Data.Common;
using Titan.Library.Domain.Users;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Contexts;

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

        command.CommandText = """
            INSERT INTO users (name, email, password_hash, password_salt, user_type)
            VALUES (@Name, @Email, @PasswordHash, @PasswordSalt, 'author')
            RETURNING id;
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

    public async Task Update(Author entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            UPDATE users
            SET name = @Name, email = @Email, password_hash = @PasswordHash, password_salt = @PasswordSalt
            WHERE id = @Id AND user_type = 'author';
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

        command.CommandText = "DELETE FROM users WHERE id = @Id AND user_type = 'author';";

        command.AddParameters(new { entity.Id });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task<IEnumerable<Author>> ToList()
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            SELECT id, name, email, password_hash, password_salt, created_at
            FROM users
            WHERE user_type = 'author';
            """;

        return await command.ExecuteListAsync(MapToAuthor);
    }

    public async Task<Author?> FindById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            SELECT id, name, email, password_hash, password_salt, created_at
            FROM users
            WHERE id = @Id AND user_type = 'author';
            """;

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToAuthor);
    }

    public async Task<Author?> FindByEmail(string email)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            SELECT id, name, email, password_hash, password_salt, created_at
            FROM users
            WHERE email = @Email AND user_type = 'author';
            """;

        command.AddParameters(new { Email = email });

        return await command.ExecuteSingleOrDefaultAsync(MapToAuthor);
    }

    private static Author MapToAuthor(DbDataReader reader) =>
        Author.Reconstitute(
            id: reader.GetInt32(reader.GetOrdinal("id")),
            name: reader.GetString(reader.GetOrdinal("name")),
            email: reader.GetString(reader.GetOrdinal("email")),
            passwordHash: reader.GetString(reader.GetOrdinal("password_hash")),
            passwordSalt: reader.GetString(reader.GetOrdinal("password_salt")),
            createdAt: reader.GetDateTime(reader.GetOrdinal("created_at"))
        );
}
