using Titan.Library.Common.Auth;
using Titan.Library.Common.Cqrs;
using Titan.Library.Common.EndPoints;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Auth;
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

        if (command.UserType.Equals(UserTypeValues.Admin, StringComparison.OrdinalIgnoreCase))
            return Result.Fail(ApplicationMessageKeys.AUTH_ADMIN_REGISTRATION_NOT_ALLOWED);

        if (
            !command.UserType.Equals(UserTypeValues.Customer, StringComparison.OrdinalIgnoreCase)
            && !command.UserType.Equals(UserTypeValues.Author, StringComparison.OrdinalIgnoreCase)
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
    private readonly IUserRepository _userRepository;
    private readonly IJwtGenerator _jwtGenerator;

    public RegisterCommandHandler(
        ICustomerRepository customerRepository,
        IAuthorRepository authorRepository,
        IJwtGenerator jwtGenerator,
        IUserRepository userRepository
    )
    {
        _customerRepository = customerRepository;
        _authorRepository = authorRepository;
        _jwtGenerator = jwtGenerator;
        _userRepository = userRepository;
    }

    protected override async Task<Result<AuthTokenDto>> InnerHandle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.FindByEmail(request.Email);
        if (user is not null)
        {
            return Result<AuthTokenDto>.Fail(ApplicationMessageKeys.USER_ALREADY_REGISTERED);
        }
        if (request.UserType.Equals(UserTypeValues.Customer, StringComparison.OrdinalIgnoreCase))
        {
            var customer = Customer.Create(request.Name, request.Email, request.Password);

            var customerId = await _customerRepository.Add(customer);
            customer.Id = customerId;

            var token = _jwtGenerator.Generate(customer.Id, UserTypeValues.Customer);
            return Result<AuthTokenDto>.Success(
                new AuthTokenDto
                {
                    Token = token,
                    UserId = customer.Id,
                    UserType = customer.RepresentUserTypeString(),
                },
                ApplicationMessageKeys.AUTH_REGISTER_SUCCESS
            );
        }
        else
        {
            var author = Author.Create(request.Name, request.Email, request.Password);
            author.SetPassword(request.Password);

            var authorId = await _authorRepository.Add(author);
            author.Id = authorId;

            var token = _jwtGenerator.Generate(author.Id, UserTypeValues.Author);
            return Result<AuthTokenDto>.Success(
                new AuthTokenDto
                {
                    Token = token,
                    UserId = author.Id,
                    UserType = author.RepresentUserTypeString(),
                },
                ApplicationMessageKeys.AUTH_REGISTER_SUCCESS
            );
        }
    }
}
