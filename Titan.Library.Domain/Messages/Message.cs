using Titan.Library.Common.Abstractions;

namespace Titan.Library.Domain.Messages;

public class Message : BaseEntity<int>
{
    private Message() { }

    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public static Message Create(string key, string value) =>
        new()
        {
            Key = key,
            Value = value,
            CreatedAt = DateTime.UtcNow,
        };

    public static Message Map(int id, string key, string value, DateTime createdAt) =>
        new()
        {
            Id = id,
            Key = key,
            Value = value,
            CreatedAt = createdAt,
        };
}
