using Titan.Library.Common.Storage;

namespace Titan.Library.Domain.Borrows;

public interface IBorrowRepository : IBaseRepository<Borrow, int>
{
    Task<IEnumerable<Borrow>> FindByCustomerId(int customerId);
    Task<Borrow?> FindActiveBorrowByCustomerAndBook(int customerId, int bookId);
}
