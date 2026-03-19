using System.Data.Common;
using Titan.Library.Domain.Borrows;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Contexts;

namespace Titan.Library.Infrastructure.Repositories;

public class BorrowRepository : IBorrowRepository
{
    private readonly ISqlDbContext _dbContext;

    public BorrowRepository(ISqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Add(Borrow entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            INSERT INTO borrows (customer_id, book_id, borrowed_at, created_at)
            VALUES (@CustomerId, @BookId, @BorrowedAt, @CreatedAt)
            RETURNING id;
            """;

        command.AddParameters(
            new
            {
                entity.CustomerId,
                entity.BookId,
                entity.BorrowedAt,
                entity.CreatedAt,
            }
        );

        return await command.ExecuteScalarValueAsync<int>();
    }

    public async Task Update(Borrow entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            UPDATE borrows
            SET returned_at = @ReturnedAt
            WHERE id = @Id;
            """;

        command.AddParameters(new { entity.Id, entity.ReturnedAt });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task Delete(Borrow entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = "DELETE FROM borrows WHERE id = @Id;";

        command.AddParameters(new { entity.Id });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task<IEnumerable<Borrow>> ToList()
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            "SELECT id, customer_id, book_id, borrowed_at, returned_at, created_at FROM borrows;";

        return await command.ExecuteListAsync(MapToBorrow);
    }

    public async Task<Borrow?> FindById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            SELECT id, customer_id, book_id, borrowed_at, returned_at, created_at
            FROM borrows
            WHERE id = @Id;
            """;

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToBorrow);
    }

    public async Task<IEnumerable<Borrow>> FindByCustomerId(int customerId)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            SELECT id, customer_id, book_id, borrowed_at, returned_at, created_at
            FROM borrows
            WHERE customer_id = @CustomerId;
            """;

        command.AddParameters(new { CustomerId = customerId });

        return await command.ExecuteListAsync(MapToBorrow);
    }

    public async Task<Borrow?> FindActiveBorrowByCustomerAndBook(int customerId, int bookId)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            SELECT id, customer_id, book_id, borrowed_at, returned_at, created_at
            FROM borrows
            WHERE customer_id = @CustomerId AND book_id = @BookId AND returned_at IS NULL
            LIMIT 1;
            """;

        command.AddParameters(new { CustomerId = customerId, BookId = bookId });

        return await command.ExecuteSingleOrDefaultAsync(MapToBorrow);
    }

    private static Borrow MapToBorrow(DbDataReader reader)
    {
        var returnedAtOrdinal = reader.GetOrdinal("returned_at");
        return new Borrow
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            CustomerId = reader.GetInt32(reader.GetOrdinal("customer_id")),
            BookId = reader.GetInt32(reader.GetOrdinal("book_id")),
            BorrowedAt = reader.GetDateTime(reader.GetOrdinal("borrowed_at")),
            ReturnedAt = reader.IsDBNull(returnedAtOrdinal)
                ? null
                : reader.GetDateTime(returnedAtOrdinal),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
        };
    }
}
