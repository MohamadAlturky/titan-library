namespace Titan.Library.Common.Auth;

public interface IJwtGenerator
{
    string Generate(int userId, string userType);
}
