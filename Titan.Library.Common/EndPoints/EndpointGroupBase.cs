using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Titan.Library.Api.Infrastructure;
using Titan.Library.Common.Results;

namespace Titan.Library.Common.EndPoints;

public abstract class EndpointGroupBase
{
    public abstract void Map(WebApplication app);

    protected static int GetUserId()
    {
        // Look for the Subject claim (sub) instead of "id"
        var userIdClaim = AppHttpContext.Current.User.Claims.FirstOrDefault(x =>
            x.Type == JwtRegisteredClaimNames.Sub || x.Type == ClaimTypes.NameIdentifier
        );

        var userId = userIdClaim?.Value ?? "0";

        return int.TryParse(userId, out var result) ? result : 0;
    }

    protected static async Task<IResult> HandleApiResponseAsync<T>(
        IApiResponseResolver apiMessageResolver,
        Result<T> result,
        CancellationToken cancellationToken = default
    )
    {
        var message = await apiMessageResolver.ResolveAsync(
            result.MessageCode ?? string.Empty,
            cancellationToken
        );
        if (result.IsSuccess)
        {
            return TypedResults.Ok(
                new SuccessApiResponse<T>
                {
                    Data = result.Data,
                    Success = result.IsSuccess,
                    Message = message,
                }
            );
        }
        else
        {
            return TypedResults.Ok(
                new SuccessApiResponse<T>
                {
                    Data = result.Data,
                    Success = result.IsSuccess,
                    Message = message,
                }
            );
        }
    }
}
