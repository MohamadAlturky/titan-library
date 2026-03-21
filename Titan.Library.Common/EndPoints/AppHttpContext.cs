using Microsoft.AspNetCore.Http;

namespace Titan.Library.Api.Infrastructure;

public static class AppHttpContext
{
    private static IHttpContextAccessor _httpContextAccessor;

    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public static HttpContext Current => _httpContextAccessor?.HttpContext;

    public static int GetUserId()
    {
        var userId = Current.User.Claims.FirstOrDefault(x => x.Type == "id")?.Value ?? "0";
        return !int.TryParse(userId, out var result) ? 0 : result;
    }
}
