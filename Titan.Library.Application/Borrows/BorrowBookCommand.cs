using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Borrows;
using Titan.Library.Domain.Books;
using Titan.Library.Domain.Borrows;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Borrows;

public class BorrowBookCommand : ICommand<BorrowDto>
{
    public int CustomerId { get; set; }
    public int BookId { get; set; }
}

public class BorrowBookCommandValidator : ICommandValidator<BorrowBookCommand, BorrowDto>
{
    public Result Validate(BorrowBookCommand command)
    {
        if (command.CustomerId <= 0)
            return Result.Fail(ApplicationMessageKeys.CUSTOMER_NOT_FOUND_FOR_BORROW);

        if (command.BookId <= 0)
            return Result.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class BorrowBookCommandHandler : BaseCommandHandler<BorrowBookCommand, BorrowDto>
{
    public override ICommandValidator<BorrowBookCommand, BorrowDto> Validator { get; set; } =
        new BorrowBookCommandValidator();

    private readonly ICustomerRepository _customerRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IBorrowRepository _borrowRepository;
    private readonly IBookTransactionHistoryRepository _historyRepository;

    public BorrowBookCommandHandler(
        ICustomerRepository customerRepository,
        IBookRepository bookRepository,
        IBorrowRepository borrowRepository,
        IBookTransactionHistoryRepository historyRepository)
    {
        _customerRepository = customerRepository;
        _bookRepository = bookRepository;
        _borrowRepository = borrowRepository;
        _historyRepository = historyRepository;
    }

    protected override async Task<Result<BorrowDto>> InnerHandle(BorrowBookCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.FindById(request.CustomerId);
        if (customer is null)
            return Result<BorrowDto>.Fail(ApplicationMessageKeys.CUSTOMER_NOT_FOUND_FOR_BORROW);

        var book = await _bookRepository.FindById(request.BookId);
        if (book is null)
            return Result<BorrowDto>.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);

        var histories = await _historyRepository.FindByBookId(book.Id);
        book.TransactionHistories = histories.ToList();

        if (!book.IsAvailable())
            return Result<BorrowDto>.Fail(ApplicationMessageKeys.BOOK_NOT_AVAILABLE);

        var existingBorrow = await _borrowRepository.FindActiveBorrowByCustomerAndBook(request.CustomerId, request.BookId);
        if (existingBorrow is not null)
            return Result<BorrowDto>.Fail(ApplicationMessageKeys.BOOK_ALREADY_BORROWED_BY_CUSTOMER);

        var borrow = Borrow.Create(request.CustomerId, request.BookId);
        var borrowId = await _borrowRepository.Add(borrow);
        borrow.Id = borrowId;

        await _historyRepository.Add(new Domain.Books.BookQuantityTransactionHistory
        {
            BookId          = request.BookId,
            Amount          = 1,
            TransactionType = Domain.Books.TransactionType.BookBorrowed,
            CreatedAt       = DateTime.UtcNow
        });

        var borrowDto = new BorrowDto();
        borrowDto.Map(borrow);

        return Result<BorrowDto>.Success(borrowDto, ApplicationMessageKeys.BORROW_CREATED_SUCCESSFULLY);
    }
}
