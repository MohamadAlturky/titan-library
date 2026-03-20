using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan.Library.Application.Books;
using Titan.Library.Common.EndPoints;
using Titan.Library.Contracts.Books;

namespace Titan.Library.Endpoints.Book;

public class BookEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this, "Books", "Books");

        group
            .MapPost("/", CreateBookAsync)
            .WithName(nameof(CreateBookAsync))
            .WithSummary("Create a new Book")
            .Produces(StatusCodes.Status200OK, typeof(BookDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapGet("/", GetBooksAsync)
            .WithName(nameof(GetBooksAsync))
            .WithSummary("Get a list of Books")
            .Produces(StatusCodes.Status200OK, typeof(IEnumerable<BookDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapGet("/{id}", GetBookByIdAsync)
            .WithName(nameof(GetBookByIdAsync))
            .WithSummary("Get a Book by Id")
            .Produces(StatusCodes.Status200OK, typeof(BookDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapGet("/isbn/{isbn}", GetBookByIsbnAsync)
            .WithName(nameof(GetBookByIsbnAsync))
            .WithSummary("Get a Book by ISBN")
            .Produces(StatusCodes.Status200OK, typeof(BookDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapPut("/{id}", UpdateBookAsync)
            .WithName(nameof(UpdateBookAsync))
            .WithSummary("Update a Book")
            .Produces(StatusCodes.Status200OK, typeof(BookDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapDelete("/{id}", DeleteBookAsync)
            .WithName(nameof(DeleteBookAsync))
            .WithSummary("Delete a Book")
            .Produces(StatusCodes.Status200OK, typeof(bool))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

    }

    private async Task<IResult> CreateBookAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromBody] CreateBookCommand request
    )
    {
        var result = await sender.Send(request);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetBooksAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [AsParameters] GetBooksQuery query
    )
    {
        var result = await sender.Send(query);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetBookByIdAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromRoute] int id
    )
    {
        var result = await sender.Send(new GetBookByIdQuery { Id = id });
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetBookByIsbnAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromRoute] string isbn
    )
    {
        var result = await sender.Send(new GetBookByIsbnQuery { Isbn = isbn });
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> UpdateBookAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromRoute] int id,
        [FromBody] UpdateBookCommand request
    )
    {
        request.Id = id;
        var result = await sender.Send(request);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> DeleteBookAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromRoute] int id
    )
    {
        var result = await sender.Send(new DeleteBookCommand { Id = id });
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }
}
