using Titan.Library.Common.Results;
using Titan.Library.Contracts.Borrows;

namespace Titan.Library.Application.Borrows.Strategies;

/// <summary>
/// Pluggable concurrency strategy for the BorrowBookCommandHandler.
/// Register exactly ONE implementation in the DI container.
/// </summary>
public interface IBorrowConcurrencyStrategy
{
    Task<Result<BorrowDto>> ExecuteAsync(
        BorrowBookCommand command,
        CancellationToken cancellationToken
    );
}
