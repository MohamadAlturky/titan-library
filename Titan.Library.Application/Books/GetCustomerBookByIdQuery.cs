using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Books;
using Titan.Library.Domain.Books;

namespace Titan.Library.Application.Books;

public class GetCustomerBookByIdQuery : IQuery<BookWithAuthorDto>
{
    public int Id { get; set; }
}

public class GetCustomerBookByIdQueryHandler : IQueryHandler<GetCustomerBookByIdQuery, BookWithAuthorDto>
{
    private readonly IBookRepository _bookRepository;

    public GetCustomerBookByIdQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<BookWithAuthorDto>> Handle(
        GetCustomerBookByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var book = await _bookRepository.GetBookWithAuthorById(request.Id);
        if (book is null)
            return Result<BookWithAuthorDto>.Fail(ApplicationMessageKeys.BOOK_NOT_FOUND);

        return Result<BookWithAuthorDto>.Success(
            BookWithAuthorDto.FromEntity(book),
            ApplicationMessageKeys.CUSTOMER_BOOK_RETRIEVED_SUCCESSFULLY
        );
    }
}
