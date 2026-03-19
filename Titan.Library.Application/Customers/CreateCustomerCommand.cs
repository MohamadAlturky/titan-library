using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Users;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Customers;

public class CreateCustomerCommand : ICommand<CustomerDto>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CreateCustomerCommandValidator : ICommandValidator<CreateCustomerCommand, CustomerDto>
{
    public Result Validate(CreateCustomerCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result.Fail(ApplicationMessageKeys.CUSTOMER_NAME_REQUIRED);

        if (string.IsNullOrWhiteSpace(command.Email))
            return Result.Fail(ApplicationMessageKeys.CUSTOMER_EMAIL_REQUIRED);

        if (string.IsNullOrWhiteSpace(command.Password))
            return Result.Fail(ApplicationMessageKeys.CUSTOMER_PASSWORD_REQUIRED);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class CreateCustomerCommandHandler : BaseCommandHandler<CreateCustomerCommand, CustomerDto>
{
    public override ICommandValidator<CreateCustomerCommand, CustomerDto> Validator { get; set; } =
        new CreateCustomerCommandValidator();

    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    protected override async Task<Result<CustomerDto>> InnerHandle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Name = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };
        customer.SetPassword(request.Password);

        var customerId = await _customerRepository.Add(customer);
        customer.Id = customerId;

        var customerDto = new CustomerDto();
        customerDto.Map(customer);

        return Result<CustomerDto>.Success(customerDto, ApplicationMessageKeys.CUSTOMER_CREATED_SUCCESSFULLY);
    }
}
