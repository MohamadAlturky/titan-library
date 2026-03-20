using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Users;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Authors;

public class GetAuthorsQuery : IQuery<List<AuthorDto>>
{
}

public class GetAuthorsQueryHandler : IQueryHandler<GetAuthorsQuery, List<AuthorDto>>
{
    private readonly IAuthorRepository _authorRepository;

    public GetAuthorsQueryHandler(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<Result<List<AuthorDto>>> Handle(GetAuthorsQuery request, CancellationToken cancellationToken)
    {
        var result = await _authorRepository.ToList();
        var response = result.Select(AuthorDto.FromEntity).ToList();

        return Result<List<AuthorDto>>.Success(response, ApplicationMessageKeys.AUTHORS_RETRIEVED_SUCCESSFULLY);
    }
}
