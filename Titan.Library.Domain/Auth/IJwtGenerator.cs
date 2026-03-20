namespace Titan.Library.Domain.Auth;

public interface IJwtGenerator
{
    string Generate(int userId, string userType);
}
