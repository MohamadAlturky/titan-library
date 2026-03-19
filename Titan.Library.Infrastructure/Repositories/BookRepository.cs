using System.Data.Common;
using Titan.Library.Domain.Books;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Configurations;
using Titan.Library.Infrastructure.Contexts;
using C = Titan.Library.Infrastructure.Configurations.BookTableConfiguration.Columns;
using T = Titan.Library.Infrastructure.Configurations.BookTableConfiguration;

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

        command.CommandText = $"""
            INSERT INTO {T.Table} ({C.Isbn}, {C.AuthorId}, {C.Title})
            VALUES (@Isbn, @AuthorId, @Title)
            RETURNING {C.Id};
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

        command.CommandText = $"""
            UPDATE {T.Table}
            SET {C.Isbn} = @Isbn, {C.AuthorId} = @AuthorId, {C.Title} = @Title
            WHERE {C.Id} = @Id;
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

        command.CommandText = $"DELETE FROM {T.Table} WHERE {C.Id} = @Id;";

        command.AddParameters(new { entity.Id });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task<IEnumerable<Book>> ToList()
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.CreatedAt} FROM {T.Table};";

        return await command.ExecuteListAsync(MapToBook);
    }

    public async Task<Book?> FindById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.CreatedAt} FROM {T.Table} WHERE {C.Id} = @Id;";

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToBook);
    }

    public async Task<Book?> FindByIsbn(string isbn)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.CreatedAt} FROM {T.Table} WHERE {C.Isbn} = @Isbn;";

        command.AddParameters(new { Isbn = isbn });

        return await command.ExecuteSingleOrDefaultAsync(MapToBook);
    }

    public async Task<IEnumerable<Book>> FindByTitle(string title)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.CreatedAt} FROM {T.Table} WHERE {C.Title} ILIKE @Title;";

        command.AddParameters(new { Title = $"%{title}%" });

        return await command.ExecuteListAsync(MapToBook);
    }

    public async Task<IEnumerable<Book>> FindByAuthorId(int authorId)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.CreatedAt} FROM {T.Table} WHERE {C.AuthorId} = @AuthorId;";

        command.AddParameters(new { AuthorId = authorId });

        return await command.ExecuteListAsync(MapToBook);
    }

    private static Book MapToBook(DbDataReader reader)
    {
        var snapshot = new BookSnapshot
        {
            Id = reader.GetInt32(reader.GetOrdinal(C.Id)),
            Isbn = reader.GetString(reader.GetOrdinal(C.Isbn)),
            AuthorId = reader.GetInt32(reader.GetOrdinal(C.AuthorId)),
            Title = reader.GetString(reader.GetOrdinal(C.Title)),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal(C.CreatedAt)),
        };
        return Book.Reconstitute(snapshot);
    }
}
