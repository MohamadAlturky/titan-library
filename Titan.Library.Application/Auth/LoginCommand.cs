using Titan.Library.Common.Auth;
using Titan.Library.Common.Cqrs;
using Titan.Library.Common.EndPoints;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Auth;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Auth;

public class LoginCommand : ICommand<AuthTokenDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
}

public class LoginCommandValidator : ICommandValidator<LoginCommand, AuthTokenDto>
{
    public Result Validate(LoginCommand command)
    {
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

public class LoginCommandHandler : BaseCommandHandler<LoginCommand, AuthTokenDto>
{
    public override ICommandValidator<LoginCommand, AuthTokenDto> Validator { get; set; } =
        new LoginCommandValidator();

    private readonly ICustomerRepository _customerRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IJwtGenerator _jwtGenerator;

    public LoginCommandHandler(
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
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        if (request.UserType.Equals(UserTypeValues.Customer, StringComparison.OrdinalIgnoreCase))
        {
            var customer = await _customerRepository.FindByEmail(request.Email);
            if (customer is null || !customer.VerifyPassword(request.Password))
                return Result<AuthTokenDto>.Fail(ApplicationMessageKeys.AUTH_INVALID_CREDENTIALS);

            var token = _jwtGenerator.Generate(customer.Id, UserTypeValues.Customer);
            return Result<AuthTokenDto>.Success(
                new AuthTokenDto
                {
                    Token = token,
                    UserId = customer.Id,
                    UserType = UserTypeValues.Customer,
                },
                ApplicationMessageKeys.AUTH_LOGIN_SUCCESS
            );
        }
        else
        {
            var author = await _authorRepository.FindByEmail(request.Email);
            if (author is null || !author.VerifyPassword(request.Password))
                return Result<AuthTokenDto>.Fail(ApplicationMessageKeys.AUTH_INVALID_CREDENTIALS);

            var token = _jwtGenerator.Generate(author.Id, UserTypeValues.Author);
            return Result<AuthTokenDto>.Success(
                new AuthTokenDto
                {
                    Token = token,
                    UserId = author.Id,
                    UserType = UserTypeValues.Author,
                },
                ApplicationMessageKeys.AUTH_LOGIN_SUCCESS
            );
        }
    }
}
