using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan.Library.Application.Authors;
using Titan.Library.Common.EndPoints;
using Titan.Library.Contracts.Users;

namespace Titan.Library.Endpoints.Author;

public class AuthorEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this, "Authors", "Authors");

        group
            .MapPost("/", CreateAuthorAsync)
            .WithName(nameof(CreateAuthorAsync))
            .WithSummary("Create a new Author")
            .Produces(StatusCodes.Status200OK, typeof(AuthorDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapGet("/", GetAuthorsAsync)
            .WithName(nameof(GetAuthorsAsync))
            .WithSummary("Get a list of Authors")
            .Produces(StatusCodes.Status200OK, typeof(IEnumerable<AuthorDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapGet("/{id}", GetAuthorByIdAsync)
            .WithName(nameof(GetAuthorByIdAsync))
            .WithSummary("Get an Author by Id")
            .Produces(StatusCodes.Status200OK, typeof(AuthorDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapDelete("/{id}", DeleteAuthorAsync)
            .WithName(nameof(DeleteAuthorAsync))
            .WithSummary("Delete an Author")
            .Produces(StatusCodes.Status200OK, typeof(bool))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));
    }

    private async Task<IResult> CreateAuthorAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromBody] CreateAuthorCommand request
    )
    {
        var result = await sender.Send(request);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetAuthorsAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [AsParameters] GetAuthorsQuery query
    )
    {
        var result = await sender.Send(query);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetAuthorByIdAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromRoute] int id
    )
    {
        var result = await sender.Send(new GetAuthorByIdQuery { Id = id });
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> DeleteAuthorAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromRoute] int id
    )
    {
        var result = await sender.Send(new DeleteAuthorCommand { Id = id });
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }
}
