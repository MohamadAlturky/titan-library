using Titan.Library.Common.Storage;

namespace Titan.Library.Domain.Users;

public interface IAdminRepository : IBaseRepository<Admin, int>
{
    Task<Admin?> FindByEmail(string email);
    Task<(List<User> items, int total)> GetUsersPaginated(string? search, int? userType, string orderBy, bool ascending, int page, int pageSize);
}
