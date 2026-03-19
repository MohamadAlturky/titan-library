using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Books;
using Titan.Library.Domain.Books;

namespace Titan.Library.Application.Books;

public class GetBookByIsbnQuery : IQuery<BookDto>
{
    public string Isbn { get; set; } = string.Empty;
}

public class GetBookByIsbnQueryHandler : IQueryHandler<GetBookByIsbnQuery, BookDto>
{
    private readonly IBookRepository _bookRepository;

    public GetBookByIsbnQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<BookDto>> Handle(GetBookByIsbnQuery request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.FindByIsbn(request.Isbn);
        if (book is null)
            return Result<BookDto>.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);

        var bookDto = new BookDto();
        bookDto.Map(book);

        return Result<BookDto>.Success(bookDto, ApplicationMessageKeys.BOOK_RETRIEVED_SUCCESSFULLY);
    }
}
