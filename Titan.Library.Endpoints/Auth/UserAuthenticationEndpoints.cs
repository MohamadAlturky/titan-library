using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Titan.Library.Application.Auth;
using Titan.Library.Common.EndPoints;
using Titan.Library.Contracts.Auth;

namespace Titan.Library.Endpoints.Auth;

public class UserAuthenticationEndpoints : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this, "auth", "Auth");

        group
            .MapPost("/login", LoginAsync)
            .WithName(nameof(LoginAsync))
            .WithSummary("Login as Customer or Author or Admin")
            .Produces(StatusCodes.Status200OK, typeof(AuthTokenDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .AllowAnonymous();

        group
            .MapPost("/register", RegisterAsync)
            .WithName(nameof(RegisterAsync))
            .WithSummary(
                "Register as Customer or Author — admin user type returns validation error"
            )
            .Produces(StatusCodes.Status200OK, typeof(AuthTokenDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .AllowAnonymous();

        group
            .MapGet("/profile", GetProfileAsync)
            .WithName(nameof(GetProfileAsync))
            .WithSummary("Get the authenticated user's profile (Customer, Author or admin)")
            .Produces(StatusCodes.Status200OK, typeof(UserProfileDto))
            .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
            .RequireUserType(UserTypeValues.Customer, UserTypeValues.Author, UserTypeValues.Admin);
    }

    private async Task<IResult> LoginAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromBody] LoginCommand request
    )
    {
        var result = await sender.Send(request);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> RegisterAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver,
        [FromBody] RegisterCommand request
    )
    {
        var result = await sender.Send(request);
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }

    private async Task<IResult> GetProfileAsync(
        [FromServices] ISender sender,
        [FromServices] IApiResponseResolver apiMessageResolver
    )
    {
        var result = await sender.Send(new GetProfileQuery { UserId = GetUserId() });
        return await HandleApiResponseAsync(apiMessageResolver, result);
    }
}
