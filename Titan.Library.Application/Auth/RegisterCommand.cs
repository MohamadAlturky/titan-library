using Titan.Library.Common.Cqrs;
using Titan.Library.Common.EndPoints;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Auth;
using Titan.Library.Domain.Auth;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Auth;

public class RegisterCommand : ICommand<AuthTokenDto>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
}

public class RegisterCommandValidator : ICommandValidator<RegisterCommand, AuthTokenDto>
{
    public Result Validate(RegisterCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result.Fail(ApplicationMessageKeys.AUTH_NAME_REQUIRED);

        if (string.IsNullOrWhiteSpace(command.Email))
            return Result.Fail(ApplicationMessageKeys.AUTH_EMAIL_REQUIRED);

        if (string.IsNullOrWhiteSpace(command.Password))
            return Result.Fail(ApplicationMessageKeys.AUTH_PASSWORD_REQUIRED);

        if (string.IsNullOrWhiteSpace(command.UserType))
            return Result.Fail(ApplicationMessageKeys.AUTH_USER_TYPE_REQUIRED);

        if (command.UserType.Equals(UserType.Admin, StringComparison.OrdinalIgnoreCase))
            return Result.Fail(ApplicationMessageKeys.AUTH_ADMIN_REGISTRATION_NOT_ALLOWED);

        if (
            !command.UserType.Equals(UserType.Customer, StringComparison.OrdinalIgnoreCase)
            && !command.UserType.Equals(UserType.Author, StringComparison.OrdinalIgnoreCase)
        )
            return Result.Fail(ApplicationMessageKeys.AUTH_INVALID_USER_TYPE);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class RegisterCommandHandler : BaseCommandHandler<RegisterCommand, AuthTokenDto>
{
    public override ICommandValidator<RegisterCommand, AuthTokenDto> Validator { get; set; } =
        new RegisterCommandValidator();

    private readonly ICustomerRepository _customerRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IJwtGenerator _jwtGenerator;

    public RegisterCommandHandler(
        ICustomerRepository customerRepository,
        IAuthorRepository authorRepository,
        IJwtGenerator jwtGenerator
    )
    {
        _customerRepository = customerRepository;
        _authorRepository = authorRepository;
        _jwtGenerator = jwtGenerator;
    }

    protected override async Task<Result<AuthTokenDto>> InnerHandle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        if (request.UserType.Equals(UserType.Customer, StringComparison.OrdinalIgnoreCase))
        {
            var customer = new Customer
            {
                Name = request.Name,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
            };
            customer.SetPassword(request.Password);

            var customerId = await _customerRepository.Add(customer);
            customer.Id = customerId;

            var token = _jwtGenerator.Generate(customer.Id, UserType.Customer);
            return Result<AuthTokenDto>.Success(
                new AuthTokenDto
                {
                    Token = token,
                    UserId = customer.Id,
                    UserType = UserType.Customer,
                },
                ApplicationMessageKeys.AUTH_REGISTER_SUCCESS
            );
        }
        else
        {
            var author = new Author
            {
                Name = request.Name,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
            };
            author.SetPassword(request.Password);

            var authorId = await _authorRepository.Add(author);
            author.Id = authorId;

            var token = _jwtGenerator.Generate(author.Id, UserType.Author);
            return Result<AuthTokenDto>.Success(
                new AuthTokenDto
                {
                    Token = token,
                    UserId = author.Id,
                    UserType = UserType.Author,
                },
                ApplicationMessageKeys.AUTH_REGISTER_SUCCESS
            );
        }
    }
}
