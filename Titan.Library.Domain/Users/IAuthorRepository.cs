using Titan.Library.Common.Storage;

namespace Titan.Library.Domain.Users;

public interface IAuthorRepository : IBaseRepository<Author, int>
{
    Task<Author?> FindByEmail(string email);
}
