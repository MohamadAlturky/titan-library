using System.Data.Common;
using Npgsql;
using NpgsqlTypes;
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
            INSERT INTO {T.Table} ({C.Isbn}, {C.AuthorId}, {C.Title}, {C.IsAvailable}, {C.IsDeleted})
            VALUES (@Isbn, @AuthorId, @Title, @IsAvailable, FALSE)
            RETURNING {C.Id};
            """;

        command.AddParameters(
            new
            {
                entity.Isbn,
                entity.AuthorId,
                entity.Title,
                entity.IsAvailable,
            }
        );

        return await command.ExecuteScalarValueAsync<int>();
    }

    public async Task Update(Book entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            UPDATE {T.Table}
            SET {C.Isbn} = @Isbn, {C.AuthorId} = @AuthorId, {C.Title} = @Title, {C.IsAvailable} = @IsAvailable
            WHERE {C.Id} = @Id AND {C.IsDeleted} = FALSE;
            """;

        command.AddParameters(
            new
            {
                entity.Id,
                entity.Isbn,
                entity.AuthorId,
                entity.Title,
                entity.IsAvailable,
            }
        );

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task Delete(Book entity)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"UPDATE {T.Table} SET {C.IsDeleted} = TRUE WHERE {C.Id} = @Id;";

        command.AddParameters(new { entity.Id });

        await command.ExecuteNonQuerySafeAsync();
    }

    public async Task<IEnumerable<Book>> ToList()
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted} FROM {T.Table} WHERE {C.IsDeleted} = FALSE;";

        return await command.ExecuteListAsync(MapToBook);
    }

    public async Task<Book?> FindById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted} FROM {T.Table} WHERE {C.Id} = @Id AND {C.IsDeleted} = FALSE;";

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToBook);
    }

    public async Task<Book?> FindByIsbn(string isbn)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted} FROM {T.Table} WHERE {C.Isbn} = @Isbn AND {C.IsDeleted} = FALSE;";

        command.AddParameters(new { Isbn = isbn });

        return await command.ExecuteSingleOrDefaultAsync(MapToBook);
    }

    public async Task<IEnumerable<Book>> FindByTitle(string title)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted} FROM {T.Table} WHERE {C.Title} ILIKE @Title AND {C.IsDeleted} = FALSE;";

        command.AddParameters(new { Title = $"%{title}%" });

        return await command.ExecuteListAsync(MapToBook);
    }

    public async Task<IEnumerable<Book>> FindByAuthorId(int authorId)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted} FROM {T.Table} WHERE {C.AuthorId} = @AuthorId AND {C.IsDeleted} = FALSE;";

        command.AddParameters(new { AuthorId = authorId });

        return await command.ExecuteListAsync(MapToBook);
    }

    // -------------------------------------------------------------------------
    // Concurrency strategy methods
    // -------------------------------------------------------------------------

    public async Task<bool> TryMarkUnavailable(int bookId)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            UPDATE {T.Table}
            SET {C.IsAvailable} = false
            WHERE {C.Id} = @Id AND {C.IsAvailable} = true;
            """;

        command.AddParameters(new { Id = bookId });

        var rows = await command.ExecuteNonQuerySafeAsync();
        return rows == 1;
    }

    public async Task<Book?> FindByIdForUpdate(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted}
            FROM {T.Table}
            WHERE {C.Id} = @Id AND {C.IsDeleted} = FALSE
            FOR UPDATE;
            """;

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToBook);
    }

    public async Task<(Book Book, long Xmin)?> FindByIdWithVersion(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted},
                   xmin::text::bigint AS row_version
            FROM {T.Table}
            WHERE {C.Id} = @Id AND {C.IsDeleted} = FALSE;
            """;

        command.AddParameters(new { Id = id });

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        var book = MapToBook(reader);
        var xmin = reader.GetInt64(reader.GetOrdinal("row_version"));
        return (book, xmin);
    }

    public async Task<bool> TryUpdateWithVersion(int bookId, long xmin)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            UPDATE {T.Table}
            SET {C.IsAvailable} = false
            WHERE {C.Id} = @Id AND xmin::text::bigint = @Xmin;
            """;

        command.AddParameters(new { Id = bookId, Xmin = xmin });

        var rows = await command.ExecuteNonQuerySafeAsync();
        return rows == 1;
    }

    public async Task<(List<Book> items, int total)> GetAuthorBooksPaginated(
        int authorId,
        string? search,
        bool? isAvailable,
        string sortColumn,
        bool ascending,
        int page,
        int pageSize
    )
    {
        await using var command = await _dbContext.CreateCommandAsync();

        var direction = ascending ? "ASC" : "DESC";
        var allowedSortColumns = new HashSet<string> { "id", "title", "isbn", "is_available" };
        var orderBy = allowedSortColumns.Contains(sortColumn) ? sortColumn : "id";
        var searchParam = search is not null ? $"%{search}%" : null;
        var offset = (page - 1) * pageSize;

        command.CommandText = $"""
            SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted},
                    COUNT(*) OVER() AS total_count
            FROM {T.Table}
            WHERE {C.AuthorId} = @AuthorId
                AND {C.IsDeleted} = FALSE
                AND (@IsAvailable::boolean IS NULL OR {C.IsAvailable} = @IsAvailable)
                AND (@Search IS NULL OR {C.Title} ILIKE @Search OR {C.Isbn} ILIKE @Search)
            ORDER BY {orderBy} {direction}
            LIMIT @PageSize OFFSET @Offset;
            """;

        command.AddParameters(
            new
            {
                AuthorId = authorId,
                IsAvailable = isAvailable,
                PageSize = pageSize,
                Offset = offset,
            }
        );
        command.Parameters.Add(
            new NpgsqlParameter("Search", NpgsqlDbType.Text)
            {
                Value = searchParam ?? (object)DBNull.Value,
            }
        );

        var items = new List<Book>();
        var total = 0;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (total == 0)
                total = reader.GetInt32(reader.GetOrdinal("total_count"));
            items.Add(MapToBook(reader));
        }

        return (items, total);
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
            IsAvailable = reader.GetBoolean(reader.GetOrdinal(C.IsAvailable)),
            IsDeleted = reader.GetBoolean(reader.GetOrdinal(C.IsDeleted)),
        };
        return Book.Reconstitute(snapshot);
    }
}
