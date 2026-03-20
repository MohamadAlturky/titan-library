using MediatR;
using Titan.Library.Application.Messages.Caching;
using Titan.Library.Domain.Messages.Events;

namespace Titan.Library.Application.Messages.Events;

public class MessageDeletedEventHandler : INotificationHandler<MessageDeletedEvent>
{
    private readonly IMessageCacheValueResolver _cacheResolver;

    public MessageDeletedEventHandler(IMessageCacheValueResolver cacheResolver)
    {
        _cacheResolver = cacheResolver;
    }

    public async Task Handle(MessageDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _cacheResolver.RemoveAsync(notification.Key, cancellationToken);
    }
}
