namespace Titan.Library.Domain.Books;

public interface IBookTransactionHistoryRepository
{
    Task<int> Add(BookQuantityTransactionHistory entity);
    Task<IEnumerable<BookQuantityTransactionHistory>> FindByBookId(int bookId);
}
