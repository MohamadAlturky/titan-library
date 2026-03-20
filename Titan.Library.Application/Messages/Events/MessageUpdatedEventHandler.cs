using MediatR;
using Titan.Library.Application.Messages.Caching;
using Titan.Library.Domain.Messages.Events;

namespace Titan.Library.Application.Messages.Events;

public class MessageUpdatedEventHandler : INotificationHandler<MessageUpdatedEvent>
{
    private readonly IMessageCacheValueResolver _cacheResolver;

    public MessageUpdatedEventHandler(IMessageCacheValueResolver cacheResolver)
    {
        _cacheResolver = cacheResolver;
    }

    public async Task Handle(MessageUpdatedEvent notification, CancellationToken cancellationToken)
    {
        await _cacheResolver.RemoveAsync(notification.Key, cancellationToken);
    }
}
