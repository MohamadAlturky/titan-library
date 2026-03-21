using Titan.Library.Common.Storage;

namespace Titan.Library.Domain.Users;

public interface IUserRepository : IBaseRepository<User, int>
{
    Task<User?> FindByEmail(string email);
}
