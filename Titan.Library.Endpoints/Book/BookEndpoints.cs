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
    }

    private static async Task<IResult> CreateBookAsync(
        [FromServices] ISender sender,
        [FromBody] CreateBookCommand request
    )
    {
        var result = await sender.Send(request);
        return HandleApiResponse(result);
    }
}
