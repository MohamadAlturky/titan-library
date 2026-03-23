using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Common.Utils;
using Titan.Library.Contracts.Books;
using Titan.Library.Domain.Books;

namespace Titan.Library.Application.AdminPanel;

public class GetAdminBooksPaginatedQuery : IQuery<PaginatedResult<BookWithAuthorDto>>
{
    public string? AuthorName { get; set; }
    public string? Search { get; set; }
    public bool? IsAvailable { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAdminBooksPaginatedQueryHandler
    : IQueryHandler<GetAdminBooksPaginatedQuery, PaginatedResult<BookWithAuthorDto>>
{
    private static readonly Dictionary<string, string> SortColumnMap = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["id"] = "id",
        ["title"] = "title",
        ["isbn"] = "isbn",
        ["isAvailable"] = "is_available",
    };

    private readonly IBookRepository _bookRepository;

    public GetAdminBooksPaginatedQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<PaginatedResult<BookWithAuthorDto>>> Handle(
        GetAdminBooksPaginatedQuery request,
        CancellationToken cancellationToken
    )
    {
        var sortColumn = SortColumnMap.GetValueOrDefault(request.SortBy ?? string.Empty, "id");
        var ascending = !string.Equals(
            request.SortDirection,
            "desc",
            StringComparison.OrdinalIgnoreCase
        );

        var (items, total) = await _bookRepository.GetAdminBooksPaginated(
            request.AuthorName,
            request.Search,
            request.IsAvailable,
            sortColumn,
            ascending,
            request.Page,
            request.PageSize
        );

        var dtos = items.Select(BookWithAuthorDto.FromEntity).ToList();
        var result = new PaginatedResult<BookWithAuthorDto>(dtos, total, request.Page, request.PageSize);

        return Result<PaginatedResult<BookWithAuthorDto>>.Success(
            result,
            ApplicationMessageKeys.ADMIN_BOOKS_RETRIEVED_SUCCESSFULLY
        );
    }
}
