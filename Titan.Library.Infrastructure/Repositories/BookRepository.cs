using System.Data.Common;
using Npgsql;
using NpgsqlTypes;
using Titan.Library.Domain.Books;
using Titan.Library.Infrastructure.AdoExtensions;
using Titan.Library.Infrastructure.Configurations;
using Titan.Library.Infrastructure.Contexts;
using C = Titan.Library.Infrastructure.Configurations.BookTableConfiguration.Columns;
using T = Titan.Library.Infrastructure.Configurations.BookTableConfiguration;
using UC = Titan.Library.Infrastructure.Configurations.UserTableConfiguration;

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
            INSERT INTO {T.Table} ({C.Isbn}, {C.AuthorId}, {C.Title}, {C.Description}, {C.IsAvailable}, {C.IsDeleted})
            VALUES (@Isbn, @AuthorId, @Title, @Description, @IsAvailable, FALSE)
            RETURNING {C.Id};
            """;

        command.AddParameters(
            new
            {
                entity.Isbn,
                entity.AuthorId,
                entity.Title,
                entity.Description,
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
            SET {C.Isbn} = @Isbn, {C.AuthorId} = @AuthorId, {C.Title} = @Title, {C.Description} = @Description, {C.IsAvailable} = @IsAvailable
            WHERE {C.Id} = @Id AND {C.IsDeleted} = FALSE;
            """;

        command.AddParameters(
            new
            {
                entity.Id,
                entity.Isbn,
                entity.AuthorId,
                entity.Title,
                entity.Description,
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
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.Description}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted} FROM {T.Table} WHERE {C.IsDeleted} = FALSE;";

        return await command.ExecuteListAsync(MapToBook);
    }

    public async Task<Book?> FindById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.Description}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted} FROM {T.Table} WHERE {C.Id} = @Id AND {C.IsDeleted} = FALSE;";

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToBook);
    }

    public async Task<Book?> FindByIsbn(string isbn)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.Description}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted} FROM {T.Table} WHERE {C.Isbn} = @Isbn AND {C.IsDeleted} = FALSE;";

        command.AddParameters(new { Isbn = isbn });

        return await command.ExecuteSingleOrDefaultAsync(MapToBook);
    }

    public async Task<IEnumerable<Book>> FindByTitle(string title)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.Description}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted} FROM {T.Table} WHERE {C.Title} ILIKE @Title AND {C.IsDeleted} = FALSE;";

        command.AddParameters(new { Title = $"%{title}%" });

        return await command.ExecuteListAsync(MapToBook);
    }

    public async Task<IEnumerable<Book>> FindByAuthorId(int authorId)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText =
            $"SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.Description}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted} FROM {T.Table} WHERE {C.AuthorId} = @AuthorId AND {C.IsDeleted} = FALSE;";

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
            SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.Description}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted}
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
            SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.Description}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted},
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
            SELECT {C.Id}, {C.Isbn}, {C.AuthorId}, {C.Title}, {C.Description}, {C.CreatedAt}, {C.IsAvailable}, {C.IsDeleted},
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

    public async Task<(List<BookWithAuthor> items, int total)> GetAdminBooksPaginated(
        string? authorName,
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
        var orderBy = allowedSortColumns.Contains(sortColumn) ? $"b.{sortColumn}" : "b.id";
        var searchParam = search is not null ? $"%{search}%" : null;
        var authorNameParam = authorName is not null ? $"%{authorName}%" : null;
        var offset = (page - 1) * pageSize;

        command.CommandText = $"""
            SELECT b.{C.Id}, b.{C.Isbn}, b.{C.AuthorId}, b.{C.Title}, b.{C.Description}, b.{C.CreatedAt}, b.{C.IsAvailable}, b.{C.IsDeleted},
                   u.name AS author_name, u.email AS author_email,
                   COUNT(*) OVER() AS total_count
            FROM {T.Table} b
            INNER JOIN {UC.Table} u ON b.{C.AuthorId} = u.id
            WHERE b.{C.IsDeleted} = FALSE
                AND (@AuthorName IS NULL OR u.name ILIKE @AuthorName)
                AND (@IsAvailable::boolean IS NULL OR b.{C.IsAvailable} = @IsAvailable)
                AND (@Search IS NULL OR b.{C.Title} ILIKE @Search OR b.{C.Isbn} ILIKE @Search)
            ORDER BY {orderBy} {direction}
            LIMIT @PageSize OFFSET @Offset;
            """;

        command.AddParameters(new { IsAvailable = isAvailable, PageSize = pageSize, Offset = offset });
        command.Parameters.Add(
            new NpgsqlParameter("AuthorName", NpgsqlDbType.Text)
            {
                Value = authorNameParam ?? (object)DBNull.Value,
            }
        );
        command.Parameters.Add(
            new NpgsqlParameter("Search", NpgsqlDbType.Text)
            {
                Value = searchParam ?? (object)DBNull.Value,
            }
        );

        var items = new List<BookWithAuthor>();
        var total = 0;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (total == 0)
                total = reader.GetInt32(reader.GetOrdinal("total_count"));
            items.Add(MapToBookWithAuthor(reader));
        }

        return (items, total);
    }

    public async Task<(List<BookWithAuthor> items, bool hasMore, int? nextCursor)> GetCustomerBooksCursor(
        string? search,
        bool? isAvailable,
        int? cursor,
        int pageSize
    )
    {
        await using var command = await _dbContext.CreateCommandAsync();

        var searchParam = search is not null ? $"%{search}%" : null;
        var fetchSize = pageSize + 1;

        command.CommandText = $"""
            SELECT b.{C.Id}, b.{C.Isbn}, b.{C.AuthorId}, b.{C.Title}, b.{C.Description}, b.{C.CreatedAt}, b.{C.IsAvailable}, b.{C.IsDeleted},
                   u.name AS author_name, u.email AS author_email
            FROM {T.Table} b
            INNER JOIN {UC.Table} u ON b.{C.AuthorId} = u.id
            WHERE b.{C.IsDeleted} = FALSE
                AND (@IsAvailable::boolean IS NULL OR b.{C.IsAvailable} = @IsAvailable)
                AND (@Search IS NULL OR b.{C.Title} ILIKE @Search OR b.{C.Isbn} ILIKE @Search)
                AND (@Cursor::int IS NULL OR b.{C.Id} > @Cursor)
            ORDER BY b.{C.Id} ASC
            LIMIT @FetchSize;
            """;

        command.AddParameters(new { IsAvailable = isAvailable, FetchSize = fetchSize });
        command.Parameters.Add(
            new NpgsqlParameter("Search", NpgsqlDbType.Text)
            {
                Value = searchParam ?? (object)DBNull.Value,
            }
        );
        command.Parameters.Add(
            new NpgsqlParameter("Cursor", NpgsqlDbType.Integer)
            {
                Value = cursor.HasValue ? cursor.Value : DBNull.Value,
            }
        );

        var items = await command.ExecuteListAsync(MapToBookWithAuthor);

        var hasMore = items.Count > pageSize;
        if (hasMore)
            items.RemoveAt(items.Count - 1);

        var nextCursor = hasMore ? items[^1].Book.Id : (int?)null;

        return (items, hasMore, nextCursor);
    }

    public async Task<BookWithAuthor?> GetBookWithAuthorById(int id)
    {
        await using var command = await _dbContext.CreateCommandAsync();

        command.CommandText = $"""
            SELECT b.{C.Id}, b.{C.Isbn}, b.{C.AuthorId}, b.{C.Title}, b.{C.Description}, b.{C.CreatedAt}, b.{C.IsAvailable}, b.{C.IsDeleted},
                   u.name AS author_name, u.email AS author_email
            FROM {T.Table} b
            INNER JOIN {UC.Table} u ON b.{C.AuthorId} = u.id
            WHERE b.{C.Id} = @Id AND b.{C.IsDeleted} = FALSE;
            """;

        command.AddParameters(new { Id = id });

        return await command.ExecuteSingleOrDefaultAsync(MapToBookWithAuthor);
    }

    private static BookWithAuthor MapToBookWithAuthor(DbDataReader reader)
    {
        var snapshot = new BookSnapshot
        {
            Id = reader.GetInt32(reader.GetOrdinal(C.Id)),
            Isbn = reader.GetString(reader.GetOrdinal(C.Isbn)),
            AuthorId = reader.GetInt32(reader.GetOrdinal(C.AuthorId)),
            Title = reader.GetString(reader.GetOrdinal(C.Title)),
            Description = reader.GetString(reader.GetOrdinal(C.Description)),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal(C.CreatedAt)),
            IsAvailable = reader.GetBoolean(reader.GetOrdinal(C.IsAvailable)),
            IsDeleted = reader.GetBoolean(reader.GetOrdinal(C.IsDeleted)),
        };
        var book = Book.Reconstitute(snapshot);
        var authorName = reader.GetString(reader.GetOrdinal("author_name"));
        var authorEmail = reader.GetString(reader.GetOrdinal("author_email"));
        return new BookWithAuthor(book, authorName, authorEmail);
    }

    private static Book MapToBook(DbDataReader reader)
    {
        var snapshot = new BookSnapshot
        {
            Id = reader.GetInt32(reader.GetOrdinal(C.Id)),
            Isbn = reader.GetString(reader.GetOrdinal(C.Isbn)),
            AuthorId = reader.GetInt32(reader.GetOrdinal(C.AuthorId)),
            Title = reader.GetString(reader.GetOrdinal(C.Title)),
            Description = reader.GetString(reader.GetOrdinal(C.Description)),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal(C.CreatedAt)),
            IsAvailable = reader.GetBoolean(reader.GetOrdinal(C.IsAvailable)),
            IsDeleted = reader.GetBoolean(reader.GetOrdinal(C.IsDeleted)),
        };
        return Book.Reconstitute(snapshot);
    }
}
