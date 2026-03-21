using Titan.Library.Common.Cqrs;
using Titan.Library.Common.EndPoints;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Auth;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.Auth;

public class GetProfileQuery : IQuery<UserProfileDto>
{
    public int UserId { get; set; }
}

public class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, UserProfileDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAuthorRepository _authorRepository;

    public GetProfileQueryHandler(
        ICustomerRepository customerRepository,
        IAuthorRepository authorRepository
    )
    {
        _customerRepository = customerRepository;
        _authorRepository = authorRepository;
    }

    public async Task<Result<UserProfileDto>> Handle(
        GetProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        var customer = await _customerRepository.FindById(request.UserId);
        if (customer is null)
            return Result<UserProfileDto>.Fail(ApplicationMessageKeys.AUTH_USER_NOT_FOUND);

        return Result<UserProfileDto>.Success(
            new UserProfileDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                UserType = UserType.Customer,
                CreatedAt = customer.CreatedAt,
                IsActive = customer.IsActive,
            },
            ApplicationMessageKeys.AUTH_PROFILE_RETRIEVED_SUCCESSFULLY
        );
    }
}
