using Titan.Library.Common.Storage;

namespace Titan.Library.Domain.Users;

public interface ICustomerRepository : IBaseRepository<Customer, int>
{
    Task<Customer?> FindByEmail(string email);
}
