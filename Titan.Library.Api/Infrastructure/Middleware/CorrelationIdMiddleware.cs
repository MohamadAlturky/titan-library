using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;
using Titan.Library.Common.Logging;

namespace Titan.Library.Api.Infrastructure.Middleware;

public sealed class CorrelationIdMiddleware : IMiddleware
{
    private const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
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
            await next(context);
        }
    }
}
