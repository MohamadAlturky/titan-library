using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Users;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Customers;

public class GetCustomersQuery : IQuery<List<CustomerDto>>
{
}

public class GetCustomersQueryHandler : IQueryHandler<GetCustomersQuery, List<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<List<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var result = await _customerRepository.ToList();
        var response = result.Select(CustomerDto.FromEntity).ToList();

        return Result<List<CustomerDto>>.Success(response, ApplicationMessageKeys.CUSTOMERS_RETRIEVED_SUCCESSFULLY);
    }
}
