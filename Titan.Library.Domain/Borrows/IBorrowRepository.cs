using Titan.Library.Common.Storage;

namespace Titan.Library.Domain.Borrows;

public interface IBorrowRepository : IBaseRepository<Borrow, int>
{
    Task<IEnumerable<Borrow>> FindByCustomerId(int customerId);
    Task<
        IEnumerable<(Borrow Borrow, string BookTitle, string AuthorName)>
    > FindByCustomerIdWithDetails(int customerId);
    Task<
        IEnumerable<(Borrow Borrow, string BookTitle, string CustomerName)>
    > FindByAuthorIdWithDetails(int authorId);
    Task<IEnumerable<(Borrow Borrow, string CustomerName)>> FindByBookIdWithDetails(int bookId);
    Task<(
        List<(Borrow Borrow, string CustomerName)> items,
        int total
    )> FindByBookIdWithDetailsPaginated(
        int bookId,
        string sortColumn,
        bool ascending,
        int page,
        int pageSize
    );
    Task<Borrow?> FindActiveBorrowByCustomerAndBook(int customerId, int bookId);
}
