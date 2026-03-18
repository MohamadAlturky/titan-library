namespace Titan.Library.Common.Abstractions;

public class BaseEntity<T>
{
    public T Id { get; set; }
    public DateTime CreatedAt { get; set; }
}
