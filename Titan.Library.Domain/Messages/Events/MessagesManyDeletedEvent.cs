using Titan.Library.Common.Cqrs;

namespace Titan.Library.Domain.Messages.Events;

public class MessagesManyDeletedEvent : IDomainEvent
{
    public IEnumerable<string> Keys { get; init; } = [];
}
