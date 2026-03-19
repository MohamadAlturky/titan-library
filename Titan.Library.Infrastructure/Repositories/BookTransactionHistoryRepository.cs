using System.Data.Common;
using Titan.Library.Domain.Books;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Contexts;
using C = Titan.Library.Infrastructure.Configurations.BookTransactionHistoryTableConfiguration.Columns;
using T = Titan.Library.Infrastructure.Configurations.BookTransactionHistoryTableConfiguration;

namespace Titan.Library.Infrastructure.Repositories;

public class BookTransactionHistoryRepository : IBookTransactionHistoryRepository
{
    private readonly ISqlDbContext _dbContext;

    public BookTransactionHistoryRepository(ISqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Add(BookQuantityTransactionHistory entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            INSERT INTO {T.Table} ({C.BookId}, {C.Amount}, {C.TransactionType}, {C.CreatedAt})
            VALUES (@BookId, @Amount, @TransactionType, @CreatedAt)
            RETURNING {C.Id};
            """;

        command.AddParameters(
            new
            {
                entity.BookId,
                entity.Amount,
                TransactionType = (int)entity.TransactionType,
                entity.CreatedAt,
            }
        );

        return await command.ExecuteScalarValueAsync<int>();
    }

    public async Task<IEnumerable<BookQuantityTransactionHistory>> FindByBookId(int bookId)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT {C.Id}, {C.BookId}, {C.Amount}, {C.TransactionType}, {C.CreatedAt}
            FROM {T.Table}
            WHERE {C.BookId} = @BookId;
            """;

        command.AddParameters(new { BookId = bookId });

        return await command.ExecuteListAsync(MapToHistory);
    }

    private static BookQuantityTransactionHistory MapToHistory(DbDataReader reader)
    {
        var snapshot = new BookTransactionHistorySnapshot
        {
            Id              = reader.GetInt32(reader.GetOrdinal(C.Id)),
            BookId          = reader.GetInt32(reader.GetOrdinal(C.BookId)),
            Amount          = reader.GetInt32(reader.GetOrdinal(C.Amount)),
            TransactionType = reader.GetInt32(reader.GetOrdinal(C.TransactionType)),
            CreatedAt       = reader.GetDateTime(reader.GetOrdinal(C.CreatedAt)),
        };
        return BookQuantityTransactionHistory.Reconstitute(snapshot);
    }
}
