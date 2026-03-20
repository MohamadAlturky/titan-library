using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Books;
using Titan.Library.Domain.Books;

namespace Titan.Library.Application.Books;

public class GetBookByIdQuery : IQuery<BookDto>
{
    public int Id { get; set; }
}

public class GetBookByIdQueryHandler : IQueryHandler<GetBookByIdQuery, BookDto>
{
    private readonly IBookRepository _bookRepository;

    public GetBookByIdQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<BookDto>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.FindById(request.Id);
        if (book is null)
            return Result<BookDto>.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);

        var bookDto = BookDto.FromEntity(book);

        return Result<BookDto>.Success(bookDto, ApplicationMessageKeys.BOOK_RETRIEVED_SUCCESSFULLY);
    }
}
