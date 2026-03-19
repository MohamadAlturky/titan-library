using System.Data.Common;
using Titan.Library.Domain.Books;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Contexts;

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

        command.CommandText = """
            INSERT INTO book_quantity_transaction_histories (book_id, amount, transaction_type, created_at)
            VALUES (@BookId, @Amount, @TransactionType, @CreatedAt)
            RETURNING id;
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

        command.CommandText = """
            SELECT id, book_id, amount, transaction_type, created_at
            FROM book_quantity_transaction_histories
            WHERE book_id = @BookId;
            """;

        command.AddParameters(new { BookId = bookId });

        return await command.ExecuteListAsync(MapToHistory);
    }

    private static BookQuantityTransactionHistory MapToHistory(DbDataReader reader) =>
        new()
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            BookId = reader.GetInt32(reader.GetOrdinal("book_id")),
            Amount = reader.GetInt32(reader.GetOrdinal("amount")),
            TransactionType = (TransactionType)
                reader.GetInt32(reader.GetOrdinal("transaction_type")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
        };
}
