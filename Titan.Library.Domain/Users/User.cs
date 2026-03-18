using Titan.Library.Common.Abstractions;

namespace Titan.Library.Domain.Users;

public class User : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
