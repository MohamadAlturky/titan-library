using Titan.Library.Common.Cqrs;

namespace Titan.Library.Domain.Messages.Events;

public class MessageDeletedEvent : IDomainEvent
{
    public string Key { get; init; } = string.Empty;
}
