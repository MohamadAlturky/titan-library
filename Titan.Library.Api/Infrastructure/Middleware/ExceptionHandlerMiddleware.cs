using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Titan.Library.Common.EndPoints;
using Titan.Library.Common.Logging;

namespace Titan.Library.Api.Infrastructure.Middleware;

public sealed class ExceptionHandlerMiddleware : IMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception ex,
        ITitanLogger<ExceptionHandlerMiddleware> logger
    )
    {
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value ?? "anonymous";

        logger.LogError(
            ex,
            "Unhandled exception. Method={RequestMethod} Path={RequestPath} Query={QueryString} UserId={UserId}",
            context.Request.Method,
            context.Request.Path.ToString(),
            context.Request.QueryString.ToString(),
            userId
        );

        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new ErrorApiResponse
        {
            Success = false,
            Message = "An unexpected error occurred.",
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var logger = context.RequestServices.GetRequiredService<
                ITitanLogger<ExceptionHandlerMiddleware>
            >();

            await HandleExceptionAsync(context, ex, logger);
        }
    }
}
