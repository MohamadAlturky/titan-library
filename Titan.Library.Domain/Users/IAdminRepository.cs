using Titan.Library.Common.Storage;

namespace Titan.Library.Domain.Users;

public interface IAdminRepository : IBaseRepository<Admin, int>
{
    Task<Admin?> FindByEmail(string email);
}
