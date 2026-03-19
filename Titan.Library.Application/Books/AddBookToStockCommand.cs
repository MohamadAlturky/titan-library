using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Domain.Books;

namespace Titan.Library.Application.Books;

public class AddBookToStockCommand : ICommand<bool>
{
    public int BookId { get; set; }
    public int Amount { get; set; }
}

public class AddBookToStockCommandValidator : ICommandValidator<AddBookToStockCommand, bool>
{
    public Result Validate(AddBookToStockCommand command)
    {
        if (command.Amount <= 0)
            return Result.Fail(ApplicationMessageKeys.STOCK_AMOUNT_MUST_BE_POSITIVE);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class AddBookToStockCommandHandler : BaseCommandHandler<AddBookToStockCommand, bool>
{
    public override ICommandValidator<AddBookToStockCommand, bool> Validator { get; set; } =
        new AddBookToStockCommandValidator();

    private readonly IBookRepository _bookRepository;
    private readonly IBookTransactionHistoryRepository _historyRepository;

    public AddBookToStockCommandHandler(
        IBookRepository bookRepository,
        IBookTransactionHistoryRepository historyRepository)
    {
        _bookRepository = bookRepository;
        _historyRepository = historyRepository;
    }

    protected override async Task<Result<bool>> InnerHandle(AddBookToStockCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.FindById(request.BookId);
        if (book is null)
            return Result<bool>.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);

        book.AddToStock(request.Amount);

        var newEntry = book.TransactionHistories.Last();
        await _historyRepository.Add(newEntry);

        return Result<bool>.Success(true, ApplicationMessageKeys.BOOK_STOCK_ADDED_SUCCESSFULLY);
    }
}
