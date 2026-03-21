using Titan.Library.Common.Auth;
using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Auth;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Auth;

public class LoginCommand : ICommand<AuthTokenDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginCommandValidator : ICommandValidator<LoginCommand, AuthTokenDto>
{
    public Result Validate(LoginCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
            return Result.Fail(ApplicationMessageKeys.AUTH_EMAIL_REQUIRED);

        if (string.IsNullOrWhiteSpace(command.Password))
            return Result.Fail(ApplicationMessageKeys.AUTH_PASSWORD_REQUIRED);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class LoginCommandHandler : BaseCommandHandler<LoginCommand, AuthTokenDto>
{
    public override ICommandValidator<LoginCommand, AuthTokenDto> Validator { get; set; } =
        new LoginCommandValidator();

    private readonly IUserRepository _userRepository;
    private readonly IJwtGenerator _jwtGenerator;

    public LoginCommandHandler(IUserRepository userRepository, IJwtGenerator jwtGenerator)
    {
        _userRepository = userRepository;
        _jwtGenerator = jwtGenerator;
    }

    protected override async Task<Result<AuthTokenDto>> InnerHandle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.FindByEmail(request.Email);
        if (user is null || !user.VerifyPassword(request.Password))
            return Result<AuthTokenDto>.Fail(ApplicationMessageKeys.AUTH_INVALID_CREDENTIALS);

        var token = _jwtGenerator.Generate(user.Id, user.RepresentUserTypeString());
        return Result<AuthTokenDto>.Success(
            new AuthTokenDto
            {
                Token = token,
                UserId = user.Id,
                UserType = user.RepresentUserTypeString(),
            },
            ApplicationMessageKeys.AUTH_LOGIN_SUCCESS
        );
    }
}
