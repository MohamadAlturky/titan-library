using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Users;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Authors;

public class CreateAuthorCommand : ICommand<AuthorDto>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CreateAuthorCommandValidator : ICommandValidator<CreateAuthorCommand, AuthorDto>
{
    public Result Validate(CreateAuthorCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result.Fail(ApplicationMessageKeys.AUTHOR_NAME_REQUIRED);

        if (string.IsNullOrWhiteSpace(command.Email))
            return Result.Fail(ApplicationMessageKeys.AUTHOR_EMAIL_REQUIRED);

        if (string.IsNullOrWhiteSpace(command.Password))
            return Result.Fail(ApplicationMessageKeys.AUTHOR_PASSWORD_REQUIRED);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class CreateAuthorCommandHandler : BaseCommandHandler<CreateAuthorCommand, AuthorDto>
{
    public override ICommandValidator<CreateAuthorCommand, AuthorDto> Validator { get; set; } =
        new CreateAuthorCommandValidator();

    private readonly IAuthorRepository _authorRepository;

    public CreateAuthorCommandHandler(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    protected override async Task<Result<AuthorDto>> InnerHandle(
        CreateAuthorCommand request,
        CancellationToken cancellationToken
    )
    {
        var author = Author.Create(request.Name, request.Email, request.Password);
        var authorId = await _authorRepository.Add(author);
        author.Id = authorId;

        var authorDto = AuthorDto.FromEntity(author);

        return Result<AuthorDto>.Success(
            authorDto,
            ApplicationMessageKeys.AUTHOR_CREATED_SUCCESSFULLY
        );
    }
}
