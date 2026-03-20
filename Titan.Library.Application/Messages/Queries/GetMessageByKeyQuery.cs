using Titan.Library.Application.Messages.Caching;
using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Messages;
using Titan.Library.Domain.Messages;

namespace Titan.Library.Application.Messages.Queries;

public class GetMessageByKeyQuery : IQuery<MessageDto>
{
    public string Key { get; set; } = string.Empty;
}

public class GetMessageByKeyQueryHandler : IQueryHandler<GetMessageByKeyQuery, MessageDto>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageCacheValueResolver _cacheResolver;

    public GetMessageByKeyQueryHandler(
        IMessageRepository messageRepository,
        IMessageCacheValueResolver cacheResolver
    )
    {
        _messageRepository = messageRepository;
        _cacheResolver = cacheResolver;
    }

    public async Task<Result<MessageDto>> Handle(
        GetMessageByKeyQuery request,
        CancellationToken cancellationToken
    )
    {
        var cached = await _cacheResolver.GetAsync(request.Key, cancellationToken);
        if (cached is not null)
            return Result<MessageDto>.Success(
                cached,
                ApplicationMessageKeys.MESSAGE_RETRIEVED_SUCCESSFULLY
            );

        var message = await _messageRepository.FindByKey(request.Key);
        if (message is null)
            return Result<MessageDto>.Fail(ApplicationMessageKeys.MESSAGE_NOT_FOUND);

        var dto = new MessageDto();
        dto.Map(message);

        await _cacheResolver.SetAsync(message.Key, dto, cancellationToken);

        return Result<MessageDto>.Success(
            dto,
            ApplicationMessageKeys.MESSAGE_RETRIEVED_SUCCESSFULLY
        );
    }
}
