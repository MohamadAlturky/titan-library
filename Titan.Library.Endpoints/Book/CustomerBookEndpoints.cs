using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan.Library.Application.Books;
using Titan.Library.Common.EndPoints;
using Titan.Library.Common.Utils;
using Titan.Library.Contracts.Books;

namespace Titan.Library.Endpoints.Book;

public class CustomerBookEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this, "CustomerBooks", "CustomerBooks");

        group
            .MapGet("/", GetCustomerBooksAsync)
            .WithName(nameof(GetCustomerBooksAsync))
            .WithSummary("Get cursor-paginated list of books with author info")
            .Produces(StatusCodes.Status200OK, typeof(CursorPaginatedResult<BookWithAuthorDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Customer);

        group
            .MapGet("/{id}", GetCustomerBookByIdAsync)
            .WithName(nameof(GetCustomerBookByIdAsync))
            .WithSummary("Get a single book with author info by id")
            .Produces(StatusCodes.Status200OK, typeof(BookWithAuthorDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Customer);
    }

    private async Task<IResult> GetCustomerBookByIdAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromRoute] int id
    )
    {
        var result = await sender.Send(new GetCustomerBookByIdQuery { Id = id });
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetCustomerBooksAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromQuery] string? search,
        [FromQuery] bool? isAvailable,
        [FromQuery] int? cursor,
        [FromQuery] int pageSize = 10
    )
    {
        GetCustomerBooksCursorQuery query = new()
        {
            Search = search,
            IsAvailable = isAvailable,
            Cursor = cursor,
            PageSize = pageSize,
        };
        var result = await sender.Send(query);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }
}
