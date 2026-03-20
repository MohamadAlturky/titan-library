using MediatR;
using Titan.Library.Application.Messages.Caching;
using Titan.Library.Domain.Messages.Events;

namespace Titan.Library.Application.Messages.Events;

public class MessagesManyDeletedEventHandler : INotificationHandler<MessagesManyDeletedEvent>
{
    private readonly IMessageCacheValueResolver _cacheResolver;

    public MessagesManyDeletedEventHandler(IMessageCacheValueResolver cacheResolver)
    {
        _cacheResolver = cacheResolver;
    }

    public async Task Handle(
        MessagesManyDeletedEvent notification,
        CancellationToken cancellationToken
    )
    {
        foreach (var key in notification.Keys)
        {
            await _cacheResolver.RemoveAsync(key, cancellationToken);
        }
    }
}
