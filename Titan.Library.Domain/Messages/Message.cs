using Titan.Library.Common.Abstractions;

namespace Titan.Library.Domain.Messages;

public class Message : BaseEntity<int>
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
