using Titan.Library.Common.Storage;

namespace Titan.Library.Domain.Books;

public interface IBookRepository : IBaseRepository<Book, int>
{
    Task<Book?> FindByIsbn(string isbn);
    Task<IEnumerable<Book>> FindByTitle(string title);
    Task<IEnumerable<Book>> FindByAuthorId(int authorId);
}