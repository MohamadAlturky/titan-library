using Titan.Library.Domain.Messages;

namespace Titan.Library.Contracts.Messages;

public class MessageDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public static MessageDto FromEntity(Message entity) => new()
    {
        Id = entity.Id, Key = entity.Key, Value = entity.Value, CreatedAt = entity.CreatedAt,
    };
}
