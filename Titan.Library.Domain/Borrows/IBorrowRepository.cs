using Titan.Library.Common.Storage;

namespace Titan.Library.Domain.Borrows;

public interface IBorrowRepository : IBaseRepository<Borrow, int>
{
    Task<IEnumerable<Borrow>> FindByCustomerId(int customerId);
    Task<IEnumerable<(Borrow Borrow, string BookTitle, string AuthorName)>> FindByCustomerIdWithDetails(int customerId);
    Task<Borrow?> FindActiveBorrowByCustomerAndBook(int customerId, int bookId);
}
