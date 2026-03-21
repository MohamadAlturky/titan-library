using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Titan.Library.Api.Infrastructure;
using Titan.Library.Common.Results;

namespace Titan.Library.Common.EndPoints;

public abstract class EndpointGroupBase
{
    public abstract void Map(WebApplication app);

    protected static int GetUserId()
    {
        var userId =
            AppHttpContext.Current.User.Claims.FirstOrDefault(x => x.Type == "id")?.Value ?? "0";
        return !int.TryParse(userId, out var result) ? 0 : result;
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
