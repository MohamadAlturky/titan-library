using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Books;
using Titan.Library.Domain.Books;

namespace Titan.Library.Application.Books;

public class GetBooksQuery : IQuery<List<BookDto>>
{
}

public class GetBooksQueryHandler : IQueryHandler<GetBooksQuery, List<BookDto>>
{
    private readonly IBookRepository _bookRepository;

    public GetBooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<List<BookDto>>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var result = await _bookRepository.ToList();
        var response = new List<BookDto>();

        foreach (var book in result)
        {
            var bookDto = new BookDto();
            bookDto.Map(book);
            response.Add(bookDto);
        }

        return Result<List<BookDto>>.Success(response, ApplicationMessageKeys.BOOKS_LIST_RETREIVED_SUCCESSFULLY);
    }
}