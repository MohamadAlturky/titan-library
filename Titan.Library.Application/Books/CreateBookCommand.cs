using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Books;
using Titan.Library.Domain.Books;

namespace Titan.Library.Application.Books;

public class CreateBookCommand : ICommand<BookDto>
{
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public int AuthorId { get; set; }
}

public class CreateBookCommandValidator : ICommandValidator<CreateBookCommand, BookDto>
{
    public Result Validate(CreateBookCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Isbn))
        {
            return Result.Fail(ApplicationMessageKeys.BOOK_ISBN_SHOULD_NOT_BE_EMPTY);
        }

        if (string.IsNullOrWhiteSpace(command.Title))
        {
            return Result.Fail(ApplicationMessageKeys.BOOK_TITLE_SHOULD_NOT_BE_EMPTY);
        }

        if (command.AuthorId <= 0)
        {
            return Result.Fail(ApplicationMessageKeys.INVALIDE_AUTHOR_ID);
        }

        return Result.Success(ApplicationMessageKeys.NO_VALIDATION_ERROR);
    }
}

public class CreateBookCommandHandler : BaseCommandHandler<CreateBookCommand, BookDto>
{
    public override ICommandValidator<CreateBookCommand, BookDto> Validator { get; set; } =
        new CreateBookCommandValidator();

    private readonly IBookRepository _bookRepository;

    public CreateBookCommandHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    protected override async Task<Result<BookDto>> InnerHandle(
        CreateBookCommand request,
        CancellationToken cancellationToken
    )
    {
        var book = new Book
        {
            Isbn = request.Isbn,
            Title = request.Title,
            AuthorId = request.AuthorId,
            CreatedAt = DateTime.Now,
        };
        var bookId = await _bookRepository.Add(book);
        book.Id = bookId;
        var bookDto = BookDto.FromEntity(book);

        return Result<BookDto>.Success(bookDto, ApplicationMessageKeys.BOOK_CREATED_SUCCESSFULLY);
    }
}
