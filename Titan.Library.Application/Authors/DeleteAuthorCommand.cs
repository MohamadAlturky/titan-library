using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Authors;

public class DeleteAuthorCommand : ICommand<bool>
{
    public int Id { get; set; }
}

public class DeleteAuthorCommandValidator : ICommandValidator<DeleteAuthorCommand, bool>
{
    public Result Validate(DeleteAuthorCommand command)
    {
        if (command.Id <= 0)
            return Result.Fail(ApplicationMessageKeys.AUTHOR_NOT_FOUND);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class DeleteAuthorCommandHandler : BaseCommandHandler<DeleteAuthorCommand, bool>
{
    public override ICommandValidator<DeleteAuthorCommand, bool> Validator { get; set; } =
        new DeleteAuthorCommandValidator();

    private readonly IAuthorRepository _authorRepository;

    public DeleteAuthorCommandHandler(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    protected override async Task<Result<bool>> InnerHandle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.FindById(request.Id);
        if (author is null)
            return Result<bool>.Fail(ApplicationMessageKeys.AUTHOR_NOT_FOUND);

        await _authorRepository.Delete(author);

        return Result<bool>.Success(true, ApplicationMessageKeys.AUTHOR_DELETED_SUCCESSFULLY);
    }
}
