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
    private readonly IUserRepository _userRepository;

    public GetProfileQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserProfileDto>> Handle(
        GetProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.FindById(request.UserId);
        if (user is null)
            return Result<UserProfileDto>.Fail(ApplicationMessageKeys.AUTH_USER_NOT_FOUND);

        return Result<UserProfileDto>.Success(
            new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserType = user.RepresentUserTypeString(),
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
            },
            ApplicationMessageKeys.AUTH_PROFILE_RETRIEVED_SUCCESSFULLY
        );
    }
}
