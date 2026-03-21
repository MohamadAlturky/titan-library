namespace Titan.Library.Contracts.Auth;

public class AuthTokenDto
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserType { get; set; } = string.Empty;
}
