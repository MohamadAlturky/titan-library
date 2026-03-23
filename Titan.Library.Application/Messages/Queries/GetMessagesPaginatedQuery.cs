using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Common.Utils;
using Titan.Library.Contracts.Messages;
using Titan.Library.Domain.Messages;

namespace Titan.Library.Application.Messages.Queries;

public class GetMessagesPaginatedQuery : IQuery<PaginatedResult<MessageDto>>
{
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetMessagesPaginatedQueryHandler
    : IQueryHandler<GetMessagesPaginatedQuery, PaginatedResult<MessageDto>>
{
    private static readonly Dictionary<string, string> SortColumnMap = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["id"] = "id",
        ["key"] = "key",
        ["value"] = "value",
        ["createdAt"] = "created_at",
    };

    private readonly IMessageRepository _messageRepository;

    public GetMessagesPaginatedQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<Result<PaginatedResult<MessageDto>>> Handle(
        GetMessagesPaginatedQuery request,
        CancellationToken cancellationToken
    )
    {
        var sortColumn = SortColumnMap.GetValueOrDefault(request.SortBy ?? string.Empty, "id");
        var ascending = !string.Equals(
            request.SortDirection,
            "desc",
            StringComparison.OrdinalIgnoreCase
        );

        var (items, total) = await _messageRepository.GetPaginated(
            request.Search,
            sortColumn,
            ascending,
            request.Page,
            request.PageSize
        );

        var dtos = items.Select(MessageDto.FromEntity).ToList();

        var result = new PaginatedResult<MessageDto>(dtos, total, request.Page, request.PageSize);

        return Result<PaginatedResult<MessageDto>>.Success(
            result,
            ApplicationMessageKeys.MESSAGES_RETRIEVED_SUCCESSFULLY
        );
    }
}
