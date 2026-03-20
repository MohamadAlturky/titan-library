using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Users;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Customers;

public class GetCustomerByIdQuery : IQuery<CustomerDto>
{
    public int Id { get; set; }
}

public class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.FindById(request.Id);
        if (customer is null)
            return Result<CustomerDto>.Fail(ApplicationMessageKeys.CUSTOMER_NOT_FOUND);

        var customerDto = CustomerDto.FromEntity(customer);

        return Result<CustomerDto>.Success(customerDto, ApplicationMessageKeys.CUSTOMER_RETRIEVED_SUCCESSFULLY);
    }
}
