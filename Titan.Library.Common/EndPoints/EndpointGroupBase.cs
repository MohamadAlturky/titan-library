using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Titan.Library.Common.Results;

namespace Titan.Library.Common.EndPoints;

public abstract class EndpointGroupBase
{
    public abstract void Map(WebApplication app);

    protected static IResult HandleApiResponse<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return TypedResults.Ok(
                new SuccessApiResponse<T>
                {
                    Data = result.Data,
                    Success = result.IsSuccess,
                    Message = result.MessageCode,
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
                    Message = result.MessageCode,
                }
            );
        }
    }
}
