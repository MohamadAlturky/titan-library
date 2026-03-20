using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Borrows;
using Titan.Library.Domain.Books;
using Titan.Library.Domain.Borrows;

namespace Titan.Library.Application.Borrows;

public class ReturnBookCommand : ICommand<BorrowDto>
{
    public int CustomerId { get; set; }
    public int BookId { get; set; }
}

public class ReturnBookCommandValidator : ICommandValidator<ReturnBookCommand, BorrowDto>
{
    public Result Validate(ReturnBookCommand command)
    {
        if (command.CustomerId <= 0)
            return Result.Fail(ApplicationMessageKeys.CUSTOMER_NOT_FOUND_FOR_BORROW);

        if (command.BookId <= 0)
            return Result.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class ReturnBookCommandHandler : BaseCommandHandler<ReturnBookCommand, BorrowDto>
{
    public override ICommandValidator<ReturnBookCommand, BorrowDto> Validator { get; set; } =
        new ReturnBookCommandValidator();

    private readonly IBorrowRepository _borrowRepository;
    private readonly IBookTransactionHistoryRepository _historyRepository;

    public ReturnBookCommandHandler(
        IBorrowRepository borrowRepository,
        IBookTransactionHistoryRepository historyRepository)
    {
        _borrowRepository = borrowRepository;
        _historyRepository = historyRepository;
    }

    protected override async Task<Result<BorrowDto>> InnerHandle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        var borrow = await _borrowRepository.FindActiveBorrowByCustomerAndBook(request.CustomerId, request.BookId);
        if (borrow is null)
            return Result<BorrowDto>.Fail(ApplicationMessageKeys.BORROW_NOT_FOUND);

        if (borrow.IsReturned)
            return Result<BorrowDto>.Fail(ApplicationMessageKeys.BOOK_ALREADY_RETURNED);

        borrow.Return();
        await _borrowRepository.Update(borrow);

        await _historyRepository.Add(new BookQuantityTransactionHistory
        {
            BookId          = request.BookId,
            Amount          = 1,
            TransactionType = TransactionType.BookReturned,
            CreatedAt       = DateTime.UtcNow
        });

        var borrowDto = BorrowDto.FromEntity(borrow);

        return Result<BorrowDto>.Success(borrowDto, ApplicationMessageKeys.BOOK_RETURNED_SUCCESSFULLY);
    }
}
