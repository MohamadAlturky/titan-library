using System.Diagnostics;
using MediatR;

namespace Titan.Library.Api.Infrastructure.Observability;

/// <summary>
/// MediatR pipeline behavior that records:
/// - library.request.duration (histogram, tagged by request.type)
/// - library.request.errors   (counter,   tagged by request.type)
/// </summary>
public sealed class MetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest).Name;
        var sw = Stopwatch.StartNew();

        try
        {
            var response = await next();
            sw.Stop();

            LibraryMetrics.RequestDuration.Record(
                sw.Elapsed.TotalMilliseconds,
                new TagList { { "request.type", requestType } });

            return response;
        }
        catch (Exception)
        {
            sw.Stop();
            LibraryMetrics.RequestErrors.Add(1,
                new TagList { { "request.type", requestType } });
            throw;
        }
    }
}
