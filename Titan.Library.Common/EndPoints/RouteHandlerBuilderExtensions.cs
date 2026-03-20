using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Titan.Library.Common.EndPoints;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder RequireUserType(
        this RouteHandlerBuilder builder,
        params string[] userTypes
    )
    {
        return builder
            .RequireAuthorization()
            .AddEndpointFilter(
                async (context, next) =>
                {
                    var user = context.HttpContext.User;

                    // 1. Check for (401)
                    if (user.Identity?.IsAuthenticated is not true)
                    {
                        return TypedResults.Unauthorized();
                    }

                    // 2. Check for (403)
                    var userTypeClaim = user.FindFirst("user_type")?.Value;

                    if (
                        string.IsNullOrEmpty(userTypeClaim)
                        || !userTypes.Contains(userTypeClaim, StringComparer.OrdinalIgnoreCase)
                    )
                    {
                        return TypedResults.Forbid();
                    }

                    return await next(context);
                }
            );
    }
}
