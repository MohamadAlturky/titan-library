using Titan.Library.Common.Storage;

namespace Titan.Library.Domain.Books;

public interface IBookRepository : IBaseRepository<Book, int>
{
    Task<Book?> FindByIsbn(string isbn);
    Task<IEnumerable<Book>> FindByTitle(string title);
    Task<IEnumerable<Book>> FindByAuthorId(int authorId);

    Task<(List<Book> items, int total)> GetAuthorBooksPaginated(
        int authorId,
        string? search,
        bool? isAvailable,
        string sortColumn,
        bool ascending,
        int page,
        int pageSize
    );

    Task<(List<BookWithAuthor> items, bool hasMore, int? nextCursor)> GetCustomerBooksCursor(
        string? search,
        bool? isAvailable,
        int? cursor,
        int pageSize
    );

    Task<BookWithAuthor?> GetBookWithAuthorById(int id);

    // --- Concurrency strategy methods ---

    /// <summary>Strategy 1 – Atomic Update: UPDATE … WHERE is_available = true</summary>
    Task<bool> TryMarkUnavailable(int bookId);

    /// <summary>Strategy 2 – Pessimistic Locking: SELECT … FOR UPDATE</summary>
    Task<Book?> FindByIdForUpdate(int id);

    /// <summary>Strategy 3 – Optimistic Locking: returns the book together with its xmin row-version.</summary>
    Task<(Book Book, long Xmin)?> FindByIdWithVersion(int id);

    /// <summary>Strategy 3 – Optimistic Locking: UPDATE … WHERE xmin = @xmin (returns false on conflict)</summary>
    Task<bool> TryUpdateWithVersion(int bookId, long xmin);
}
