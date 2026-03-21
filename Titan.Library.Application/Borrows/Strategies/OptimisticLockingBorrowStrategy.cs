using Titan.Library.Common.Results;
using Titan.Library.Common.Storage;
using Titan.Library.Contracts.Borrows;
using Titan.Library.Domain.Books;
using Titan.Library.Domain.Borrows;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Borrows.Strategies;

/// <summary>
/// Strategy 3 – Optimistic Locking (PostgreSQL xmin row version)
///
/// Does NOT lock the row on read. Instead, it captures the row's internal
/// version token (xmin) alongside the book data. When it is time to write,
/// the UPDATE includes the xmin in the WHERE clause. If another transaction
/// modified the row in between, PostgreSQL's xmin has advanced → 0 rows
/// updated → conflict detected.
///
///   Read:   SELECT …, xmin::text::bigint AS row_version FROM books WHERE id = @Id
///   Write:  UPDATE books SET is_available = false
///           WHERE id = @Id AND xmin::text::bigint = @Xmin
///
/// Best for: low-contention scenarios (conflicts are rare). Avoids any blocking
/// and has minimal overhead under normal load.
/// </summary>
public class OptimisticLockingBorrowStrategy : IBorrowConcurrencyStrategy
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IBorrowRepository _borrowRepository;
    private readonly IAsyncUnitOfWork _unitOfWork;

    public OptimisticLockingBorrowStrategy(
        ICustomerRepository customerRepository,
        IBookRepository bookRepository,
        IBorrowRepository borrowRepository,
        IAsyncUnitOfWork unitOfWork
    )
    {
        _customerRepository = customerRepository;
        _bookRepository = bookRepository;
        _borrowRepository = borrowRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BorrowDto>> ExecuteAsync(
        BorrowBookCommand command,
        CancellationToken cancellationToken
    )
    {
        var customer = await _customerRepository.FindById(command.CustomerId);
        if (customer is null)
            return Result<BorrowDto>.Fail(ApplicationMessageKeys.CUSTOMER_NOT_FOUND_FOR_BORROW);

        // Read the book together with its current xmin version token.
        // No lock is taken — this read is completely non-blocking.
        var versionedBook = await _bookRepository.FindByIdWithVersion(command.BookId);
        if (versionedBook is null)
            return Result<BorrowDto>.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);

        var (book, xmin) = versionedBook.Value;

        if (!book.IsAvailable)
            return Result<BorrowDto>.Fail(ApplicationMessageKeys.BOOK_NOT_AVAILABLE);

        var existingBorrow = await _borrowRepository.FindActiveBorrowByCustomerAndBook(
            command.CustomerId,
            command.BookId
        );
        if (existingBorrow is not null)
            return Result<BorrowDto>.Fail(ApplicationMessageKeys.BOOK_ALREADY_BORROWED_BY_CUSTOMER);

        // Begin transaction only for the write phase.
        await _unitOfWork.BeginTransactionAsync(ct: cancellationToken);
        try
        {
            // Conditional UPDATE: only succeeds if the xmin has NOT changed since we read it.
            // If another transaction modified the book row, xmin is now different → 0 rows → conflict.
            var updated = await _bookRepository.TryUpdateWithVersion(command.BookId, xmin);
            if (!updated)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return Result<BorrowDto>.Fail(ApplicationMessageKeys.BORROW_CONCURRENCY_CONFLICT);
            }

            var borrow = Borrow.Create(command.CustomerId, command.BookId);
            var borrowId = await _borrowRepository.Add(borrow);
            borrow.Id = borrowId;

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result<BorrowDto>.Success(
                BorrowDto.FromEntity(borrow),
                ApplicationMessageKeys.BORROW_CREATED_SUCCESSFULLY
            );
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
