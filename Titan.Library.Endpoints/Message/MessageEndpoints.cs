using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan.Library.Application.Messages.Commands;
using Titan.Library.Application.Messages.Queries;
using Titan.Library.Common.EndPoints;
using Titan.Library.Common.Utils;
using Titan.Library.Contracts.Messages;

namespace Titan.Library.Endpoints.Message;

public class MessageEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this, "Messages", "Messages");

        group
            .MapGet("/", GetMessagesPaginatedAsync)
            .WithName(nameof(GetMessagesPaginatedAsync))
            .WithSummary("Get paginated list of Messages")
            .Produces(StatusCodes.Status200OK, typeof(PaginatedResult<MessageDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapGet("/{key}", GetMessageByKeyAsync)
            .WithName(nameof(GetMessageByKeyAsync))
            .WithSummary("Get a Message by Id")
            .Produces(StatusCodes.Status200OK, typeof(MessageDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapPost("/", CreateMessageAsync)
            .WithName(nameof(CreateMessageAsync))
            .WithSummary("Create a new Message")
            .Produces(StatusCodes.Status200OK, typeof(MessageDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapPut("/{id}", UpdateMessageAsync)
            .WithName(nameof(UpdateMessageAsync))
            .WithSummary("Update a Message")
            .Produces(StatusCodes.Status200OK, typeof(MessageDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapDelete("/{id}", DeleteMessageAsync)
            .WithName(nameof(DeleteMessageAsync))
            .WithSummary("Delete a Message")
            .Produces(StatusCodes.Status200OK, typeof(bool))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));
    }

    private async Task<IResult> GetMessagesPaginatedAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [AsParameters] GetMessagesPaginatedQuery query
    )
    {
        var result = await sender.Send(query);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetMessageByKeyAsync(
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromServices] ISender sender,
        [FromRoute] string key
    )
    {
        var result = await sender.Send(new GetMessageByKeyQuery { Key = key });
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> CreateMessageAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromBody] CreateMessageCommand request
    )
    {
        var result = await sender.Send(request);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> UpdateMessageAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromRoute] int id,
        [FromBody] UpdateMessageCommand request
    )
    {
        request.Id = id;
        var result = await sender.Send(request);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> DeleteMessageAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromRoute] int id
    )
    {
        var result = await sender.Send(new DeleteMessageCommand { Id = id });
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }
}
