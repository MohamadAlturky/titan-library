using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan.Library.Application.Books;
using Titan.Library.Application.Borrows;
using Titan.Library.Common.EndPoints;
using Titan.Library.Contracts.Borrows;

namespace Titan.Library.Endpoints.Borrow;

public class CustomerBorrowEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this, "CustomerBorrows", "CustomerBorrows");

        group
            .MapPost("/borrow/{bookId}", BorrowBookAsync)
            .WithName(nameof(BorrowBookAsync))
            .WithSummary("Borrow a book")
            .Produces(StatusCodes.Status200OK, typeof(BorrowDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Customer);

        group
            .MapPost("/return/{bookId}", ReturnBookAsync)
            .WithName(nameof(ReturnBookAsync))
            .WithSummary("Return a borrowed book")
            .Produces(StatusCodes.Status200OK, typeof(BorrowDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Customer);

        group
            .MapGet("/", GetBorrowsAsync)
            .WithName(nameof(GetBorrowsAsync))
            .WithSummary("Get all borrows")
            .Produces(StatusCodes.Status200OK, typeof(IEnumerable<BorrowDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapGet("/Mine", GetBorrowsByCustomerAsync)
            .WithName(nameof(GetBorrowsByCustomerAsync))
            .WithSummary("Get borrows by customer")
            .Produces(StatusCodes.Status200OK, typeof(IEnumerable<BorrowDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Customer);
    }

    private async Task<IResult> BorrowBookAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromRoute] int bookId
    )
    {
        var request = new BorrowBookCommand() { BookId = bookId, CustomerId = GetUserId() };
        var result = await sender.Send(request);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> ReturnBookAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromRoute] int bookId
    )
    {
        ReturnBookCommand request = new() { BookId = bookId, CustomerId = GetUserId() };
        var result = await sender.Send(request);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetBorrowsAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [AsParameters] GetBorrowsQuery query
    )
    {
        var result = await sender.Send(query);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetBorrowsByCustomerAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver
    )
    {
        var result = await sender.Send(new GetBorrowsByCustomerQuery { CustomerId = GetUserId() });
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }
}
