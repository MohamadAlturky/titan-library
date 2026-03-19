using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Customers;

public class DeleteCustomerCommand : ICommand<bool>
{
    public int Id { get; set; }
}

public class DeleteCustomerCommandValidator : ICommandValidator<DeleteCustomerCommand, bool>
{
    public Result Validate(DeleteCustomerCommand command)
    {
        if (command.Id <= 0)
            return Result.Fail(ApplicationMessageKeys.CUSTOMER_NOT_FOUND);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class DeleteCustomerCommandHandler : BaseCommandHandler<DeleteCustomerCommand, bool>
{
    public override ICommandValidator<DeleteCustomerCommand, bool> Validator { get; set; } =
        new DeleteCustomerCommandValidator();

    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    protected override async Task<Result<bool>> InnerHandle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.FindById(request.Id);
        if (customer is null)
            return Result<bool>.Fail(ApplicationMessageKeys.CUSTOMER_NOT_FOUND);

        await _customerRepository.Delete(customer);

        return Result<bool>.Success(true, ApplicationMessageKeys.CUSTOMER_DELETED_SUCCESSFULLY);
    }
}
