using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan.Library.Application.Borrows;
using Titan.Library.Common.EndPoints;
using Titan.Library.Contracts.Borrows;

namespace Titan.Library.Endpoints.Borrow;

public class BorrowEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this, "Borrows", "Borrows");

        group
            .MapPost("/borrow", BorrowBookAsync)
            .WithName(nameof(BorrowBookAsync))
            .WithSummary("Borrow a book")
            .Produces(StatusCodes.Status200OK, typeof(BorrowDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapPost("/return", ReturnBookAsync)
            .WithName(nameof(ReturnBookAsync))
            .WithSummary("Return a borrowed book")
            .Produces(StatusCodes.Status200OK, typeof(BorrowDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapGet("/", GetBorrowsAsync)
            .WithName(nameof(GetBorrowsAsync))
            .WithSummary("Get all borrows")
            .Produces(StatusCodes.Status200OK, typeof(IEnumerable<BorrowDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));

        group
            .MapGet("/customer/{customerId}", GetBorrowsByCustomerAsync)
            .WithName(nameof(GetBorrowsByCustomerAsync))
            .WithSummary("Get borrows by customer")
            .Produces(StatusCodes.Status200OK, typeof(IEnumerable<BorrowDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails));
    }

    private static async Task<IResult> BorrowBookAsync(
        [FromServices] ISender sender,
        [FromBody] BorrowBookCommand request
    )
    {
        var result = await sender.Send(request);
        return HandleApiResponse(result);
    }

    private static async Task<IResult> ReturnBookAsync(
        [FromServices] ISender sender,
        [FromBody] ReturnBookCommand request
    )
    {
        var result = await sender.Send(request);
        return HandleApiResponse(result);
    }

    private static async Task<IResult> GetBorrowsAsync(
        [FromServices] ISender sender,
        [AsParameters] GetBorrowsQuery query
    )
    {
        var result = await sender.Send(query);
        return HandleApiResponse(result);
    }

    private static async Task<IResult> GetBorrowsByCustomerAsync(
        [FromServices] ISender sender,
        [FromRoute] int customerId
    )
    {
        var result = await sender.Send(new GetBorrowsByCustomerQuery { CustomerId = customerId });
        return HandleApiResponse(result);
    }
}
