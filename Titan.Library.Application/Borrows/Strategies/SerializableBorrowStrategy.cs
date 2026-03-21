using System.Data;
using System.Data.Common;
using Titan.Library.Common.Results;
using Titan.Library.Common.Storage;
using Titan.Library.Contracts.Borrows;
using Titan.Library.Domain.Books;
using Titan.Library.Domain.Borrows;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Borrows.Strategies;

/// <summary>
/// Strategy 4 – Serializable Isolation Level
///
/// Writes completely normal SELECT / INSERT / UPDATE statements with zero manual
/// locking code. The database engine itself acts as the referee: when it detects
/// that two concurrent transactions would produce a non-serializable result, it
/// aborts one of them with a "serialization failure" (SQLSTATE 40001).
///
/// The handler catches that specific error and retries the entire operation from
/// scratch (up to <see cref="MaxRetries"/> times).
///
/// Best for: complex operations that touch multiple tables simultaneously, where
/// writing explicit locks would be error-prone and risk deadlocks.
/// </summary>
public class SerializableBorrowStrategy : IBorrowConcurrencyStrategy
{
    private const int MaxRetries = 3;
    private const string SerializationFailureSqlState = "40001";

    private readonly ICustomerRepository _customerRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IBorrowRepository _borrowRepository;
    private readonly IAsyncUnitOfWork _unitOfWork;

    public SerializableBorrowStrategy(
        ICustomerRepository customerRepository,
        IBookRepository bookRepository,
        IBorrowRepository borrowRepository,
        IAsyncUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _bookRepository = bookRepository;
        _borrowRepository = borrowRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BorrowDto>> ExecuteAsync(BorrowBookCommand command, CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
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

                if (!book.IsAvailable)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return Result<BorrowDto>.Fail(ApplicationMessageKeys.BOOK_NOT_AVAILABLE);
                }

                var existingBorrow = await _borrowRepository.FindActiveBorrowByCustomerAndBook(
                    command.CustomerId, command.BookId);
                if (existingBorrow is not null)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return Result<BorrowDto>.Fail(ApplicationMessageKeys.BOOK_ALREADY_BORROWED_BY_CUSTOMER);
                }

                var borrow = Borrow.Create(command.CustomerId, command.BookId);
                var borrowId = await _borrowRepository.Add(borrow);
                borrow.Id = borrowId;

                book.IsAvailable = false;
                await _bookRepository.Update(book);

                await _unitOfWork.CommitAsync(cancellationToken);
                return Result<BorrowDto>.Success(BorrowDto.FromEntity(borrow), ApplicationMessageKeys.BORROW_CREATED_SUCCESSFULLY);
            }
            catch (DbException ex) when (ex.SqlState == SerializationFailureSqlState)
            {
                // PostgreSQL detected a concurrency anomaly and aborted this transaction.
                // The transaction is already rolled back by the database at this point.
                await _unitOfWork.RollbackAsync(cancellationToken);

                if (attempt == MaxRetries)
                    return Result<BorrowDto>.Fail(ApplicationMessageKeys.BORROW_CONCURRENCY_CONFLICT);

                // Retry the entire operation from the beginning.
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }

        // Unreachable — kept to satisfy the compiler.
        return Result<BorrowDto>.Fail(ApplicationMessageKeys.BORROW_CONCURRENCY_CONFLICT);
    }
}
