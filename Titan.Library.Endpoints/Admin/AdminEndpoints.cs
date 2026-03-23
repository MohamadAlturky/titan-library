using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan.Library.Application.AdminPanel;
using Titan.Library.Common.EndPoints;
using Titan.Library.Common.Utils;
using Titan.Library.Contracts.Admin;
using Titan.Library.Contracts.Books;
using Titan.Library.Contracts.Borrows;

namespace Titan.Library.Endpoints.Admin;

public class AdminEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this, "Admin", "Admin");

        group
            .MapGet("/users", GetAdminUsersPaginatedAsync)
            .WithName(nameof(GetAdminUsersPaginatedAsync))
            .WithSummary("Get paginated list of users (customers and authors)")
            .Produces(StatusCodes.Status200OK, typeof(PaginatedResult<AdminUserDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Admin);

        group
            .MapGet("/books", GetAdminBooksPaginatedAsync)
            .WithName(nameof(GetAdminBooksPaginatedAsync))
            .WithSummary("Get paginated list of books")
            .Produces(StatusCodes.Status200OK, typeof(PaginatedResult<BookWithAuthorDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Admin);

        group
            .MapGet("/books/{bookId}/borrows", GetBookBorrowsAsync)
            .WithName(nameof(GetBookBorrowsAsync))
            .WithSummary("Get paginated borrow history for a book")
            .Produces(StatusCodes.Status200OK, typeof(PaginatedResult<BorrowDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Admin);
    }

    private async Task<IResult> GetAdminUsersPaginatedAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [AsParameters] GetAdminUsersPaginatedQuery query
    )
    {
        var result = await sender.Send(query);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetAdminBooksPaginatedAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [AsParameters] GetAdminBooksPaginatedQuery query
    )
    {
        var result = await sender.Send(query);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetBookBorrowsAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromRoute] int bookId,
        [AsParameters] GetBookBorrowsQuery query
    )
    {
        query.BookId = bookId;
        var result = await sender.Send(query);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }
}
