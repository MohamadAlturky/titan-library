using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan.Library.Application.Cache;
using Titan.Library.Common.EndPoints;
using Titan.Library.Common.Utils; // ScanResult<T>
using Titan.Library.Contracts.Cache;

namespace Titan.Library.Endpoints.Cache;

public class CacheEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this, "Cache", "Cache");

        group
            .MapPost("/records", CreateCacheRecordAsync)
            .WithName(nameof(CreateCacheRecordAsync))
            .WithSummary("Create a new cache record stored in Redis")
            .Produces(StatusCodes.Status200OK, typeof(CacheRecordDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapGet("/records", GetCacheRecordsAsync)
            .WithName(nameof(GetCacheRecordsAsync))
            .WithSummary(
                "Get cache records using Redis SCAN cursor — only the requested batch is fetched"
            )
            .Produces(StatusCodes.Status200OK, typeof(ScanResult<CacheRecordDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));
    }

    private async Task<IResult> CreateCacheRecordAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromBody] CreateCacheRecordCommand command
    )
    {
        var result = await sender.Send(command);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetCacheRecordsAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [AsParameters] GetCacheRecordsQuery query
    )
    {
        var result = await sender.Send(query);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }
}
