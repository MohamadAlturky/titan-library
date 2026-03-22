using System.Text.Json.Serialization;
using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Common.Utils;
using Titan.Library.Contracts.Books;
using Titan.Library.Domain.Books;

namespace Titan.Library.Application.Books;

public class GetAuthorBooksPaginatedQuery : IQuery<PaginatedResult<BookDto>>
{
    public int AuthorId { get; set; }

    public string? Search { get; set; }
    public bool? IsAvailable { get; set; }

    public string? SortBy { get; set; }

    public string? SortDirection { get; set; } = "asc";

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAuthorBooksPaginatedQueryHandler
    : IQueryHandler<GetAuthorBooksPaginatedQuery, PaginatedResult<BookDto>>
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

    public GetAuthorBooksPaginatedQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<PaginatedResult<BookDto>>> Handle(
        GetAuthorBooksPaginatedQuery request,
        CancellationToken cancellationToken
    )
    {
        var sortColumn = SortColumnMap.GetValueOrDefault(request.SortBy ?? string.Empty, "id");
        var ascending = !string.Equals(
            request.SortDirection,
            "desc",
            StringComparison.OrdinalIgnoreCase
        );

        var (items, total) = await _bookRepository.GetAuthorBooksPaginated(
            request.AuthorId,
            request.Search,
            request.IsAvailable,
            sortColumn,
            ascending,
            request.Page,
            request.PageSize
        );

        var dtos = items.Select(BookDto.FromEntity).ToList();
        var result = new PaginatedResult<BookDto>(dtos, total, request.Page, request.PageSize);

        return Result<PaginatedResult<BookDto>>.Success(
            result,
            ApplicationMessageKeys.AUTHOR_BOOKS_RETRIEVED_SUCCESSFULLY
        );
    }
}
