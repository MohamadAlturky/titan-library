using System.Data.Common;
using Titan.Library.Domain.Users;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Contexts;

namespace Titan.Library.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ISqlDbContext _dbContext;

    public CustomerRepository(ISqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Add(Customer entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            INSERT INTO users (name, email, password_hash, password_salt, user_type)
            VALUES (@Name, @Email, @PasswordHash, @PasswordSalt, 'customer')
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

    public async Task Update(Customer entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            UPDATE users
            SET name = @Name, email = @Email, password_hash = @PasswordHash, password_salt = @PasswordSalt
            WHERE id = @Id AND user_type = 'customer';
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

    public async Task Delete(Customer entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = "DELETE FROM users WHERE id = @Id AND user_type = 'customer';";

        command.AddParameters(new { entity.Id });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task<IEnumerable<Customer>> ToList()
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            SELECT id, name, email, password_hash, password_salt, created_at
            FROM users
            WHERE user_type = 'customer';
            """;

        return await command.ExecuteListAsync(MapToCustomer);
    }

    public async Task<Customer?> FindById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            SELECT id, name, email, password_hash, password_salt, created_at
            FROM users
            WHERE id = @Id AND user_type = 'customer';
            """;

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToCustomer);
    }

    public async Task<Customer?> FindByEmail(string email)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            SELECT id, name, email, password_hash, password_salt, created_at
            FROM users
            WHERE email = @Email AND user_type = 'customer';
            """;

        command.AddParameters(new { Email = email });

        return await command.ExecuteSingleOrDefaultAsync(MapToCustomer);
    }

    private static Customer MapToCustomer(DbDataReader reader) =>
        Customer.Reconstitute(
            id: reader.GetInt32(reader.GetOrdinal("id")),
            name: reader.GetString(reader.GetOrdinal("name")),
            email: reader.GetString(reader.GetOrdinal("email")),
            passwordHash: reader.GetString(reader.GetOrdinal("password_hash")),
            passwordSalt: reader.GetString(reader.GetOrdinal("password_salt")),
            createdAt: reader.GetDateTime(reader.GetOrdinal("created_at"))
        );
}
