using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Borrows;
using Titan.Library.Domain.Borrows;

namespace Titan.Library.Application.Borrows;

public class GetBorrowsByAuthorQuery : IQuery<List<BorrowDto>>
{
    public int AuthorId { get; set; }
}

public class GetBorrowsByAuthorQueryHandler
    : IQueryHandler<GetBorrowsByAuthorQuery, List<BorrowDto>>
{
    private readonly IBorrowRepository _borrowRepository;

    public GetBorrowsByAuthorQueryHandler(IBorrowRepository borrowRepository)
    {
        _borrowRepository = borrowRepository;
    }

    public async Task<Result<List<BorrowDto>>> Handle(
        GetBorrowsByAuthorQuery request,
        CancellationToken cancellationToken
    )
    {
        var result = await _borrowRepository.FindByAuthorIdWithDetails(request.AuthorId);
        var response = result
            .Select(r =>
            {
                var dto = BorrowDto.FromEntity(r.Borrow);
                dto.BookTitle = r.BookTitle;
                dto.CustomerName = r.CustomerName;
                return dto;
            })
            .ToList();

        return Result<List<BorrowDto>>.Success(
            response,
            ApplicationMessageKeys.AUTHOR_BORROWS_RETRIEVED_SUCCESSFULLY
        );
    }
}
