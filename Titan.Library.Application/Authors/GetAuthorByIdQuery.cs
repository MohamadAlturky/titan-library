using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Users;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Authors;

public class GetAuthorByIdQuery : IQuery<AuthorDto>
{
    public int Id { get; set; }
}

public class GetAuthorByIdQueryHandler : IQueryHandler<GetAuthorByIdQuery, AuthorDto>
{
    private readonly IAuthorRepository _authorRepository;

    public GetAuthorByIdQueryHandler(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result<AuthorDto>> Handle(GetAuthorByIdQuery request, CancellationToken cancellationToken)
    {
        var author = await _authorRepository.FindById(request.Id);
        if (author is null)
            return Result<AuthorDto>.Fail(ApplicationMessageKeys.AUTHOR_NOT_FOUND);

        var authorDto = new AuthorDto();
        authorDto.Map(author);

        return Result<AuthorDto>.Success(authorDto, ApplicationMessageKeys.AUTHOR_RETRIEVED_SUCCESSFULLY);
    }
}
