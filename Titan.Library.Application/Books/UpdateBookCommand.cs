using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Books;
using Titan.Library.Domain.Books;

namespace Titan.Library.Application.Books;

public class UpdateBookCommand : ICommand<BookDto>
{
    public int Id { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AuthorId { get; set; }
}

public class UpdateBookCommandValidator : ICommandValidator<UpdateBookCommand, BookDto>
{
    public Result Validate(UpdateBookCommand command)
    {
        if (command.Id <= 0)
            return Result.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);

        if (string.IsNullOrWhiteSpace(command.Isbn))
            return Result.Fail(ApplicationMessageKeys.BOOK_ISBN_SHOULD_NOT_BE_EMPTY);

        if (string.IsNullOrWhiteSpace(command.Title))
            return Result.Fail(ApplicationMessageKeys.BOOK_TITLE_SHOULD_NOT_BE_EMPTY);

        if (string.IsNullOrWhiteSpace(command.Description))
        {
            return Result.Fail(ApplicationMessageKeys.BOOK_DESCRIPTION_SHOULD_NOT_BE_EMPTY);
        }
        if (command.Description.Length < 50)
        {
            return Result.Fail(ApplicationMessageKeys.BOOK_DESCRIPTION_SHOULD_NOT_BE_EMPTY);
        }

        if (command.AuthorId <= 0)
            return Result.Fail(ApplicationMessageKeys.INVALIDE_AUTHOR_ID);

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class UpdateBookCommandHandler : BaseCommandHandler<UpdateBookCommand, BookDto>
{
    public override ICommandValidator<UpdateBookCommand, BookDto> Validator { get; set; } =
        new UpdateBookCommandValidator();

    private readonly IBookRepository _bookRepository;

    public UpdateBookCommandHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    protected override async Task<Result<BookDto>> InnerHandle(
        UpdateBookCommand request,
        CancellationToken cancellationToken
    )
    {
        var book = await _bookRepository.FindById(request.Id);
        if (book is null)
            return Result<BookDto>.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);

        book.Isbn = request.Isbn;
        book.Title = request.Title;
        book.Description = request.Description;
        book.AuthorId = request.AuthorId;

        await _bookRepository.Update(book);

        var bookDto = BookDto.FromEntity(book);

        return Result<BookDto>.Success(bookDto, ApplicationMessageKeys.BOOK_UPDATED_SUCCESSFULLY);
    }
}
