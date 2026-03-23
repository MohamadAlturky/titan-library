using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan.Library.Application.Feedbacks;
using Titan.Library.Common.EndPoints;
using Titan.Library.Contracts.Feedbacks;

namespace Titan.Library.Endpoints.Feedback;

public class FeedbackEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this, "Feedbacks", "Feedbacks");

        group
            .MapPost("/", SubmitFeedbackAsync)
            .WithName(nameof(SubmitFeedbackAsync))
            .WithSummary("Submit feedback")
            .Produces(StatusCodes.Status200OK, typeof(FeedbackDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Customer);

        group
            .MapGet("/", GetFeedbacksAsync)
            .WithName(nameof(GetFeedbacksAsync))
            .WithSummary("Get all feedbacks")
            .Produces(StatusCodes.Status200OK, typeof(IEnumerable<FeedbackDto>))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Admin);
    }

    private async Task<IResult> SubmitFeedbackAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromBody] SubmitFeedbackRequest body
    )
    {
        var command = new SubmitFeedbackCommand
        {
            CustomerId = GetUserId(),
            Category = body.Category,
            Rating = body.Rating,
            Subject = body.Subject,
            Message = body.Message,
        };

        var result = await sender.Send(command);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetFeedbacksAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver
    )
    {
        var result = await sender.Send(new GetFeedbacksQuery());
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }
}

public record SubmitFeedbackRequest(
    string Category,
    int? Rating,
    string Subject,
    string Message
);
