using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Borrows;
using Titan.Library.Domain.Borrows;

namespace Titan.Library.Application.Borrows;

public class GetBorrowsQuery : IQuery<List<BorrowDto>>
{
}

public class GetBorrowsQueryHandler : IQueryHandler<GetBorrowsQuery, List<BorrowDto>>
{
    private readonly IBorrowRepository _borrowRepository;

    public GetBorrowsQueryHandler(IBorrowRepository borrowRepository)
    {
        _borrowRepository = borrowRepository;
    }

    public async Task<Result<List<BorrowDto>>> Handle(GetBorrowsQuery request, CancellationToken cancellationToken)
    {
        var result = await _borrowRepository.ToList();
        var response = result.Select(BorrowDto.FromEntity).ToList();

        return Result<List<BorrowDto>>.Success(response, ApplicationMessageKeys.BORROWS_RETRIEVED_SUCCESSFULLY);
    }
}
