using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Domain.Books;

namespace Titan.Library.Application.Books;

public class DeleteBookCommand : ICommand<bool>
{
    public int Id { get; set; }
}

public class DeleteBookCommandValidator : ICommandValidator<DeleteBookCommand, bool>
{
    public Result Validate(DeleteBookCommand command)
    {
        if (command.Id <= 0)
            return Result.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class DeleteBookCommandHandler : BaseCommandHandler<DeleteBookCommand, bool>
{
    public override ICommandValidator<DeleteBookCommand, bool> Validator { get; set; } =
        new DeleteBookCommandValidator();

    private readonly IBookRepository _bookRepository;

    public DeleteBookCommandHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    protected override async Task<Result<bool>> InnerHandle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.FindById(request.Id);
        if (book is null)
            return Result<bool>.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);

        await _bookRepository.Delete(book);

        return Result<bool>.Success(true, ApplicationMessageKeys.BOOK_DELETED_SUCCESSFULLY);
    }
}
