using System.Data;
using System.Data.Common;
using Titan.Library.Domain.Books;
using Titan.Library.Infrastructure.Connectors;
using Titan.Library.Infrastructure.Contexts;
using Titan.Library.Infrastructure.AdoExtensions;

namespace Titan.Library.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly ISqlDbContext _dbContext;

    public BookRepository(ISqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Add(Book entity)
    {
        // Note: Assuming _dbContext exposes a way to create a command. 
        // Adjust to `_dbContext.Connection.CreateCommand()` if needed based on your interface.
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = @"
            INSERT INTO Books (Isbn, AuthorId, Title) 
            VALUES (@Isbn, @AuthorId, @Title);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        command.AddParameters(new
        {
            entity.Isbn,
            entity.AuthorId,
            entity.Title
        });

        // Using your async scalar extension to get the newly generated ID
        return await command.ExecuteScalarValueAsync<int>();
    }

    public async Task Update(Book entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = @"
            UPDATE Books 
            SET Isbn = @Isbn, AuthorId = @AuthorId, Title = @Title 
            WHERE Id = @Id;";

        command.AddParameters(new
        {
            entity.Id, // Inherited from BaseEntity<int>
            entity.Isbn,
            entity.AuthorId,
            entity.Title
        });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task Delete(Book entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = "DELETE FROM Books WHERE Id = @Id;";

        command.AddParameters(new { entity.Id });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task<IEnumerable<Book>> ToList()
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = "SELECT Id, Isbn, AuthorId, Title FROM Books;";

        return await command.ExecuteListAsync(MapToBook);
    }

    public async Task<Book?> FindById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = "SELECT Id, Isbn, AuthorId, Title FROM Books WHERE Id = @Id;";

        command.AddParameters(new { Id = id });

        var book = await command.ExecuteSingleOrDefaultAsync(MapToBook);

        return book;
    }

    // --- Helper Methods ---

    private static Book MapToBook(DbDataReader reader)
    {
        return new Book
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Isbn = reader.GetString(reader.GetOrdinal("Isbn")),
            AuthorId = reader.GetInt32(reader.GetOrdinal("AuthorId")),
            Title = reader.GetString(reader.GetOrdinal("Title"))
        };
    }
}