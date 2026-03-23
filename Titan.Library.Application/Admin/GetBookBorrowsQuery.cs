using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Common.Utils;
using Titan.Library.Contracts.Borrows;
using Titan.Library.Domain.Borrows;

namespace Titan.Library.Application.AdminPanel;

public class GetBookBorrowsQuery : IQuery<PaginatedResult<BorrowDto>>
{
    public int BookId { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetBookBorrowsQueryHandler : IQueryHandler<GetBookBorrowsQuery, PaginatedResult<BorrowDto>>
{
    private static readonly Dictionary<string, string> SortColumnMap = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["id"] = "br.id",
        ["customerName"] = "cu.name",
        ["createdAt"] = "br.created_at",
        ["returnedAt"] = "br.returned_at",
        ["isReturned"] = "br.is_returned",
    };

    private readonly IBorrowRepository _borrowRepository;

    public GetBookBorrowsQueryHandler(IBorrowRepository borrowRepository)
    {
        _borrowRepository = borrowRepository;
    }

    public async Task<Result<PaginatedResult<BorrowDto>>> Handle(
        GetBookBorrowsQuery request,
        CancellationToken cancellationToken
    )
    {
        var sortColumn = SortColumnMap.GetValueOrDefault(request.SortBy ?? string.Empty, "br.created_at");
        var ascending = !string.Equals(request.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        var (items, total) = await _borrowRepository.FindByBookIdWithDetailsPaginated(
            request.BookId,
            sortColumn,
            ascending,
            request.Page,
            request.PageSize
        );

        var dtos = items
            .Select(r =>
            {
                var dto = BorrowDto.FromEntity(r.Borrow);
                dto.CustomerName = r.CustomerName;
                return dto;
            })
            .ToList();

        var result = new PaginatedResult<BorrowDto>(dtos, total, request.Page, request.PageSize);

        return Result<PaginatedResult<BorrowDto>>.Success(
            result,
            ApplicationMessageKeys.ADMIN_BOOK_BORROWS_RETRIEVED_SUCCESSFULLY
        );
    }
}
