using Titan.Library.Common.Dtos;
using Titan.Library.Domain.Messages;

namespace Titan.Library.Contracts.Messages;

public class MessageDto : BaseDto<Message, int>
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public override void Map(Message entity)
    {
        Id = entity.Id;
        Key = entity.Key;
        Value = entity.Value;
        CreatedAt = entity.CreatedAt;
    }
}
