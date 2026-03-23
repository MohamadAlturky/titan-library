using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan.Library.Application.Borrows;
using Titan.Library.Common.EndPoints;
using Titan.Library.Contracts.Borrows;

namespace Titan.Library.Endpoints.Borrow;

public class AuthorBorrowEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this, "AuthorBorrows", "AuthorBorrows");

        group
            .MapGet("/Mine", GetBorrowsByAuthorAsync)
            .WithName(nameof(GetBorrowsByAuthorAsync))
            .WithSummary("Get borrows of the author's books")
            .Produces(StatusCodes.Status200OK, typeof(IEnumerable<BorrowDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Author);
    }

    private async Task<IResult> GetBorrowsByAuthorAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver
    )
    {
        var result = await sender.Send(new GetBorrowsByAuthorQuery { AuthorId = GetUserId() });
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }
}
