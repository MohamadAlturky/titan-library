using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Titan.Library.Application.Books;
using Titan.Library.Common.EndPoints;
using Titan.Library.Common.Utils;
using Titan.Library.Contracts.Books;

namespace Titan.Library.Endpoints.Book;

public class AuthorBookEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this, "AuthorBooks", "AuthorBooks");

        group
            .MapGet("/", GetAuthorBooksAsync)
            .WithName(nameof(GetAuthorBooksAsync))
            .WithSummary("Get paginated list of author's books")
            .Produces(StatusCodes.Status200OK, typeof(PaginatedResult<BookDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Author);

        group
            .MapPost("/", CreateBookAsync)
            .WithName(nameof(CreateBookAsync))
            .WithSummary("Create a new book")
            .Produces(StatusCodes.Status200OK, typeof(BookDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Author);

        group
            .MapPut("/{id}", UpdateBookAsync)
            .WithName(nameof(UpdateBookAsync))
            .WithSummary("Update a book")
            .Produces(StatusCodes.Status200OK, typeof(BookDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Author);

        group
            .MapDelete("/{id}", DeleteBookAsync)
            .WithName(nameof(DeleteBookAsync))
            .WithSummary("Mark a book as deleted")
            .Produces(StatusCodes.Status200OK, typeof(bool))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Author);
    }

    private async Task<IResult> GetAuthorBooksAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromQuery] string? search,
        [FromQuery] bool? isAvailable,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDirection,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10
    )
    {
        GetAuthorBooksPaginatedQuery query = new()
        {
            AuthorId = GetUserId(),
            IsAvailable = isAvailable,
            Page = page,
            PageSize = pageSize,
            Search = search,
            SortBy = sortBy,
            SortDirection = sortDirection,
        };
        var result = await sender.Send(query);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> CreateBookAsync(
        [FromServices] ISender sender,
        [FromServices] ILogger<AuthorBookEndpoints> logger,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromBody] CreateBookCommand request
    )
    {
        request.AuthorId = GetUserId();
        logger.LogInformation("Author Id {AuthorId}", request.AuthorId);
        logger.LogInformation("GetUserId {GetUserId}", GetUserId());
        var result = await sender.Send(request);
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
        request.AuthorId = GetUserId();
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
