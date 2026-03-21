using System.Diagnostics.Metrics;

namespace Titan.Library.Api.Infrastructure.Observability;

public static class LibraryMetrics
{
    public const string MeterName = "Titan.Library";

    private static readonly Meter Meter = new(MeterName, "1.0.0");

    /// <summary>MediatR handler processing time in ms, tagged by request.type.</summary>
    public static readonly Histogram<double> RequestDuration =
        Meter.CreateHistogram<double>("library.request.duration", "ms",
            "Duration of MediatR handler execution");

    /// <summary>MediatR handlers that threw an exception, tagged by request.type.</summary>
    public static readonly Counter<long> RequestErrors =
        Meter.CreateCounter<long>("library.request.errors", "errors",
            "Number of failed MediatR handler executions");
}
