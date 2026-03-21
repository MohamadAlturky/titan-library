using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;
using Titan.Library.Common.Logging;

namespace Titan.Library.Api.Infrastructure.Middleware;

public sealed class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var provider = context.RequestServices.GetRequiredService<ICorrelationIdProvider>();

        var incoming = context.Request.Headers[HeaderName].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(incoming))
            provider.Set(incoming);

        var correlationId = provider.CorrelationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
