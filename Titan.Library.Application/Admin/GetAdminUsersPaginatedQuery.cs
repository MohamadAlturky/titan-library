using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Common.Utils;
using Titan.Library.Contracts.Admin;
using Titan.Library.Domain.Users;

namespace Titan.Library.Application.AdminPanel;

public class GetAdminUsersPaginatedQuery : IQuery<PaginatedResult<AdminUserDto>>
{
    public string? Search { get; set; }
    public int? UserType { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAdminUsersPaginatedQueryHandler
    : IQueryHandler<GetAdminUsersPaginatedQuery, PaginatedResult<AdminUserDto>>
{
    private static readonly Dictionary<string, string> SortColumnMap = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["id"] = "id",
        ["name"] = "name",
        ["email"] = "email",
        ["createdAt"] = "created_at",
        ["isActive"] = "is_active",
    };

    private readonly IAdminRepository _adminRepository;

    public GetAdminUsersPaginatedQueryHandler(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<Result<PaginatedResult<AdminUserDto>>> Handle(
        GetAdminUsersPaginatedQuery request,
        CancellationToken cancellationToken
    )
    {
        var sortColumn = SortColumnMap.GetValueOrDefault(request.SortBy ?? string.Empty, "id");
        var ascending = !string.Equals(
            request.SortDirection,
            "desc",
            StringComparison.OrdinalIgnoreCase
        );

        var (items, total) = await _adminRepository.GetUsersPaginated(
            request.Search,
            request.UserType,
            sortColumn,
            ascending,
            request.Page,
            request.PageSize
        );

        var dtos = items.Select(AdminUserDto.FromEntity).ToList();
        var result = new PaginatedResult<AdminUserDto>(dtos, total, request.Page, request.PageSize);

        return Result<PaginatedResult<AdminUserDto>>.Success(
            result,
            ApplicationMessageKeys.ADMIN_USERS_RETRIEVED_SUCCESSFULLY
        );
    }
}
