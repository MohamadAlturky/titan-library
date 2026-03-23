using System.Data.Common;
using Titan.Library.Domain.Borrows;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Contexts;
using BC = Titan.Library.Infrastructure.Configurations.BookTableConfiguration.Columns;
using BT = Titan.Library.Infrastructure.Configurations.BookTableConfiguration;
using C = Titan.Library.Infrastructure.Configurations.BorrowTableConfiguration.Columns;
using T = Titan.Library.Infrastructure.Configurations.BorrowTableConfiguration;
using UC = Titan.Library.Infrastructure.Configurations.UserTableConfiguration.Columns;
using UT = Titan.Library.Infrastructure.Configurations.UserTableConfiguration;

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

        command.CommandText = $"""
            INSERT INTO {T.Table} ({C.CustomerId}, {C.BookId}, {C.CreatedAt})
            VALUES (@CustomerId, @BookId, @CreatedAt)
            RETURNING {C.Id};
            """;

        command.AddParameters(
            new
            {
                entity.CustomerId,
                entity.BookId,
                entity.CreatedAt,
            }
        );

        return await command.ExecuteScalarValueAsync<int>();
    }

    public async Task Update(Borrow entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            UPDATE {T.Table}
            SET {C.ReturnedAt} = @ReturnedAt, {C.IsReturned} = @IsReturned
            WHERE {C.Id} = @Id;
            """;

        command.AddParameters(
            new
            {
                entity.Id,
                entity.ReturnedAt,
                entity.IsReturned,
            }
        );

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task Delete(Borrow entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"DELETE FROM {T.Table} WHERE {C.Id} = @Id;";

        command.AddParameters(new { entity.Id });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task<IEnumerable<Borrow>> ToList()
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.CustomerId}, {C.BookId}, {C.IsReturned}, {C.ReturnedAt}, {C.CreatedAt} FROM {T.Table};";

        return await command.ExecuteListAsync(MapToBorrow);
    }

    public async Task<Borrow?> FindById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT {C.Id}, {C.CustomerId}, {C.BookId}, {C.IsReturned}, {C.ReturnedAt}, {C.CreatedAt}
            FROM {T.Table}
            WHERE {C.Id} = @Id;
            """;

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToBorrow);
    }

    public async Task<IEnumerable<Borrow>> FindByCustomerId(int customerId)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT {C.Id}, {C.CustomerId}, {C.BookId}, {C.IsReturned}, {C.ReturnedAt}, {C.CreatedAt}
            FROM {T.Table}
            WHERE {C.CustomerId} = @CustomerId;
            """;

        command.AddParameters(new { CustomerId = customerId });

        return await command.ExecuteListAsync(MapToBorrow);
    }

    public async Task<
        IEnumerable<(Borrow Borrow, string BookTitle, string AuthorName)>
    > FindByCustomerIdWithDetails(int customerId)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT br.{C.Id}, br.{C.CustomerId}, br.{C.BookId}, br.{C.IsReturned}, br.{C.ReturnedAt}, br.{C.CreatedAt},
                   b.{BC.Title}  AS book_title,
                   u.{UC.Name}   AS author_name
            FROM {T.Table} br
            INNER JOIN {BT.Table} b ON br.{C.BookId}  = b.{BC.Id}
            INNER JOIN {UT.Table} u ON b.{BC.AuthorId} = u.{UC.Id}
            WHERE br.{C.CustomerId} = @CustomerId
            ORDER BY br.{C.CreatedAt} DESC;
            """;

        command.AddParameters(new { CustomerId = customerId });

        var results = new List<(Borrow, string, string)>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(
                (
                    MapToBorrow(reader),
                    reader.GetString(reader.GetOrdinal("book_title")),
                    reader.GetString(reader.GetOrdinal("author_name"))
                )
            );
        }
        return results;
    }

    public async Task<Borrow?> FindActiveBorrowByCustomerAndBook(int customerId, int bookId)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT {C.Id}, {C.CustomerId}, {C.BookId}, {C.IsReturned}, {C.ReturnedAt}, {C.CreatedAt}
            FROM {T.Table}
            WHERE {C.CustomerId} = @CustomerId AND {C.BookId} = @BookId AND {C.ReturnedAt} IS NULL
            LIMIT 1;
            """;

        command.AddParameters(new { CustomerId = customerId, BookId = bookId });

        return await command.ExecuteSingleOrDefaultAsync(MapToBorrow);
    }

    private static Borrow MapToBorrow(DbDataReader reader)
    {
        var returnedAtOrdinal = reader.GetOrdinal(C.ReturnedAt);
        var snapshot = new BorrowSnapshot
        {
            Id = reader.GetInt32(reader.GetOrdinal(C.Id)),
            CustomerId = reader.GetInt32(reader.GetOrdinal(C.CustomerId)),
            BookId = reader.GetInt32(reader.GetOrdinal(C.BookId)),
            IsReturned = reader.GetBoolean(reader.GetOrdinal(C.IsReturned)),
            ReturnedAt = reader.IsDBNull(returnedAtOrdinal)
                ? null
                : reader.GetDateTime(returnedAtOrdinal),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal(C.CreatedAt)),
        };
        return Borrow.Reconstitute(snapshot);
    }
}
