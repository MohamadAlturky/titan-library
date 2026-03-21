using Titan.Library.Common.Results;
using Titan.Library.Common.Storage;
using Titan.Library.Contracts.Borrows;
using Titan.Library.Domain.Books;
using Titan.Library.Domain.Borrows;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Borrows.Strategies;

/// <summary>
/// Strategy 2 – Pessimistic Locking (SELECT … FOR UPDATE)
///
/// Acquires an exclusive row-level lock on the book row the moment it is read.
/// Any other transaction that attempts to read the same row FOR UPDATE is physically
/// blocked at the database level until this transaction commits or rolls back.
///
///   SELECT … FROM books WHERE id = @Id FOR UPDATE
///
/// Flow:
///   BEGIN TRANSACTION
///     SELECT … FOR UPDATE  ← User B is blocked here until User A finishes
///     (C# validation)
///     INSERT borrow
///     UPDATE book (is_available = false)
///   COMMIT  ← User B is unblocked, reads is_available = false, and is rejected
///
/// Best for: workflows that require complex C# validation between the read and
/// the write, where you need to guarantee the data cannot change mid-operation.
/// </summary>
public class PessimisticLockingBorrowStrategy : IBorrowConcurrencyStrategy
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IBorrowRepository _borrowRepository;
    private readonly IAsyncUnitOfWork _unitOfWork;

    public PessimisticLockingBorrowStrategy(
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
        await _unitOfWork.BeginTransactionAsync(ct: cancellationToken);
        try
        {
            var customer = await _customerRepository.FindById(command.CustomerId);
            if (customer is null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return Result<BorrowDto>.Fail(ApplicationMessageKeys.CUSTOMER_NOT_FOUND_FOR_BORROW);
            }

            // FOR UPDATE: the row is locked for the duration of this transaction.
            // Concurrent requests block here rather than reading stale data.
            var book = await _bookRepository.FindByIdForUpdate(command.BookId);
            if (book is null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return Result<BorrowDto>.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);
            }

            if (!book.IsAvailable)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return Result<BorrowDto>.Fail(ApplicationMessageKeys.BOOK_NOT_AVAILABLE);
            }

            var existingBorrow = await _borrowRepository.FindActiveBorrowByCustomerAndBook(
                command.CustomerId,
                command.BookId
            );
            if (existingBorrow is not null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return Result<BorrowDto>.Fail(
                    ApplicationMessageKeys.BOOK_ALREADY_BORROWED_BY_CUSTOMER
                );
            }

            var borrow = Borrow.Create(command.CustomerId, command.BookId);
            var borrowId = await _borrowRepository.Add(borrow);
            borrow.Id = borrowId;

            book.IsAvailable = false;
            await _bookRepository.Update(book);

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
