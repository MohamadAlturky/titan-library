using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Common.Utils;
using Titan.Library.Contracts.Messages;
using Titan.Library.Domain.Messages;

namespace Titan.Library.Application.Messages.Queries;

public class GetMessagesPaginatedQuery : IQuery<PaginatedResult<MessageDto>>
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetMessagesPaginatedQueryHandler
    : IQueryHandler<GetMessagesPaginatedQuery, PaginatedResult<MessageDto>>
{
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
        var (items, total) = await _messageRepository.GetPaginated(
            request.Search,
            request.Page,
            request.PageSize
        );

        var dtos = items
            .Select(m =>
            {
                var dto = new MessageDto();
                dto.Map(m);
                return dto;
            })
            .ToList();

        var result = new PaginatedResult<MessageDto>(dtos, total, request.Page, request.PageSize);

        return Result<PaginatedResult<MessageDto>>.Success(
            result,
            ApplicationMessageKeys.MESSAGES_RETRIEVED_SUCCESSFULLY
        );
    }
}
