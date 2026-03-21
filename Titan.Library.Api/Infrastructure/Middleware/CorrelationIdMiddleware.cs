using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;
using Titan.Library.Common.Logging;

namespace Titan.Library.Api.Infrastructure.Middleware;

public sealed class CorrelationIdMiddleware : IMiddleware
{
    private const string CorrelationHeader = "X-Correlation-ID";
    private const string TraceHeader       = "X-Trace-ID";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var provider = context.RequestServices.GetRequiredService<ICorrelationIdProvider>();

        var incoming = context.Request.Headers[CorrelationHeader].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(incoming))
        {
            provider.Set(incoming);
        }
        else
        {
            var traceId = Activity.Current?.TraceId.ToString();
            if (!string.IsNullOrWhiteSpace(traceId))
                provider.Set(traceId);
        }

        var correlationId = provider.CorrelationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationHeader] = correlationId;

            var activeTraceId = Activity.Current?.TraceId.ToString();
            if (!string.IsNullOrWhiteSpace(activeTraceId))
                context.Response.Headers[TraceHeader] = activeTraceId;

            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
