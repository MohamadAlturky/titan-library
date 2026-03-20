using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Borrows;
using Titan.Library.Domain.Borrows;

namespace Titan.Library.Application.Borrows;

public class GetBorrowsByCustomerQuery : IQuery<List<BorrowDto>>
{
    public int CustomerId { get; set; }
}

public class GetBorrowsByCustomerQueryHandler : IQueryHandler<GetBorrowsByCustomerQuery, List<BorrowDto>>
{
    private readonly IBorrowRepository _borrowRepository;

    public GetBorrowsByCustomerQueryHandler(IBorrowRepository borrowRepository)
    {
        _borrowRepository = borrowRepository;
    }

    public async Task<Result<List<BorrowDto>>> Handle(GetBorrowsByCustomerQuery request, CancellationToken cancellationToken)
    {
        var result = await _borrowRepository.FindByCustomerId(request.CustomerId);
        var response = result.Select(BorrowDto.FromEntity).ToList();

        return Result<List<BorrowDto>>.Success(response, ApplicationMessageKeys.CUSTOMER_BORROWS_RETRIEVED_SUCCESSFULLY);
    }
}
