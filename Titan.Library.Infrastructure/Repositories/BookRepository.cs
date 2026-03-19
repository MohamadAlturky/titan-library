using System.Data.Common;
using Titan.Library.Domain.Books;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Contexts;

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
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            INSERT INTO books (isbn, author_id, title)
            VALUES (@Isbn, @AuthorId, @Title)
            RETURNING id;
            """;

        command.AddParameters(
            new
            {
                entity.Isbn,
                entity.AuthorId,
                entity.Title,
            }
        );

        return await command.ExecuteScalarValueAsync<int>();
    }

    public async Task Update(Book entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = """
            UPDATE books
            SET isbn = @Isbn, author_id = @AuthorId, title = @Title
            WHERE id = @Id;
            """;

        command.AddParameters(
            new
            {
                entity.Id,
                entity.Isbn,
                entity.AuthorId,
                entity.Title,
            }
        );

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task Delete(Book entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = "DELETE FROM books WHERE id = @Id;";

        command.AddParameters(new { entity.Id });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task<IEnumerable<Book>> ToList()
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = "SELECT id, isbn, author_id, title, created_at FROM books;";

        return await command.ExecuteListAsync(MapToBook);
    }

    public async Task<Book?> FindById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            "SELECT id, isbn, author_id, title, created_at FROM books WHERE id = @Id;";

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToBook);
    }

    public async Task<Book?> FindByIsbn(string isbn)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            "SELECT id, isbn, author_id, title, created_at FROM books WHERE isbn = @Isbn;";

        command.AddParameters(new { Isbn = isbn });

        return await command.ExecuteSingleOrDefaultAsync(MapToBook);
    }

    public async Task<IEnumerable<Book>> FindByTitle(string title)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            "SELECT id, isbn, author_id, title, created_at FROM books WHERE title ILIKE @Title;";

        command.AddParameters(new { Title = $"%{title}%" });

        return await command.ExecuteListAsync(MapToBook);
    }

    public async Task<IEnumerable<Book>> FindByAuthorId(int authorId)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            "SELECT id, isbn, author_id, title, created_at FROM books WHERE author_id = @AuthorId;";

        command.AddParameters(new { AuthorId = authorId });

        return await command.ExecuteListAsync(MapToBook);
    }

    private static Book MapToBook(DbDataReader reader) =>
        new()
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Isbn = reader.GetString(reader.GetOrdinal("isbn")),
            AuthorId = reader.GetInt32(reader.GetOrdinal("author_id")),
            Title = reader.GetString(reader.GetOrdinal("title")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
        };
}
