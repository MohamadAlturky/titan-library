using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Common.Utils;
using Titan.Library.Contracts.Books;
using Titan.Library.Domain.Books;

namespace Titan.Library.Application.Books;

public class GetCustomerBooksCursorQuery : IQuery<CursorPaginatedResult<BookWithAuthorDto>>
{
    public string? Search { get; set; }
    public bool? IsAvailable { get; set; }
    public int? Cursor { get; set; }
    public int PageSize { get; set; } = 10;
}

public class GetCustomerBooksCursorQueryHandler
    : IQueryHandler<GetCustomerBooksCursorQuery, CursorPaginatedResult<BookWithAuthorDto>>
{
    private readonly IBookRepository _bookRepository;

    public GetCustomerBooksCursorQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<CursorPaginatedResult<BookWithAuthorDto>>> Handle(
        GetCustomerBooksCursorQuery request,
        CancellationToken cancellationToken
    )
    {
        var (items, hasMore, nextCursor) = await _bookRepository.GetCustomerBooksCursor(
            request.Search,
            request.IsAvailable,
            request.Cursor,
            request.PageSize
        );

        var dtos = items.Select(BookWithAuthorDto.FromEntity).ToList();
        var result = new CursorPaginatedResult<BookWithAuthorDto>(dtos, hasMore, nextCursor);

        return Result<CursorPaginatedResult<BookWithAuthorDto>>.Success(
            result,
            ApplicationMessageKeys.CUSTOMER_BOOKS_RETRIEVED_SUCCESSFULLY
        );
    }
}
