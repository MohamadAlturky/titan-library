using Titan.Library.Application.Borrows.Strategies;
using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Borrows;

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

/// <summary>
/// Single MediatR handler that delegates all concurrency logic to the
/// registered <see cref="IBorrowConcurrencyStrategy"/>.
///
/// To switch strategies, change the DI registration in
/// <c>ApplicationBootstrapper.AddApplication</c>:
///
///   services.AddScoped&lt;IBorrowConcurrencyStrategy, AtomicUpdateBorrowStrategy&gt;();       // Strategy 1
///   services.AddScoped&lt;IBorrowConcurrencyStrategy, PessimisticLockingBorrowStrategy&gt;();  // Strategy 2
///   services.AddScoped&lt;IBorrowConcurrencyStrategy, OptimisticLockingBorrowStrategy&gt;();   // Strategy 3
///   services.AddScoped&lt;IBorrowConcurrencyStrategy, SerializableBorrowStrategy&gt;();        // Strategy 4
/// </summary>
public class BorrowBookCommandHandler : BaseCommandHandler<BorrowBookCommand, BorrowDto>
{
    public override ICommandValidator<BorrowBookCommand, BorrowDto> Validator { get; set; } =
        new BorrowBookCommandValidator();

    private readonly IBorrowConcurrencyStrategy _strategy;

    public BorrowBookCommandHandler(IBorrowConcurrencyStrategy strategy)
    {
        _strategy = strategy;
    }

    protected override Task<Result<BorrowDto>> InnerHandle(
        BorrowBookCommand request,
        CancellationToken cancellationToken
    ) => _strategy.ExecuteAsync(request, cancellationToken);
}
