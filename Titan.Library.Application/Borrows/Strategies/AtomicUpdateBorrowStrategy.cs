using Titan.Library.Common.Results;
using Titan.Library.Common.Storage;
using Titan.Library.Contracts.Borrows;
using Titan.Library.Domain.Books;
using Titan.Library.Domain.Borrows;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Borrows.Strategies;

/// <summary>
/// Strategy 1 – Atomic Update
///
/// Replaces the two-step "read → check → update" pattern with a single conditional
/// UPDATE statement that acts as both the availability check and the update in one
/// database round-trip.
///
///   UPDATE books SET is_available = false
///   WHERE id = @Id AND is_available = true
///
/// If the book was grabbed by another request between our FindById and this UPDATE,
/// the WHERE clause returns 0 rows → the handler fails safely with no dirty state.
///
/// Best for: high-contention scenarios where the availability check is simple.
/// </summary>
public class AtomicUpdateBorrowStrategy : IBorrowConcurrencyStrategy
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IBorrowRepository _borrowRepository;
    private readonly IAsyncUnitOfWork _unitOfWork;

    public AtomicUpdateBorrowStrategy(
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

            var book = await _bookRepository.FindById(command.BookId);
            if (book is null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return Result<BorrowDto>.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);
            }

            // Single atomic UPDATE: check + mark unavailable in one shot.
            // Another request grabbing this book between FindById and here is safe —
            // the WHERE is_available = true condition will simply return 0 rows.
            var marked = await _bookRepository.TryMarkUnavailable(command.BookId);
            if (!marked)
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
