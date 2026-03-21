using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace Titan.Library.Api.Infrastructure.Observability;

/// <summary>
/// Enriches Serilog log events with the current OpenTelemetry trace and span IDs,
/// enabling correlation between structured logs and distributed traces.
/// </summary>
public sealed class ActivityEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var activity = Activity.Current;
        if (activity is null) return;

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("TraceId", activity.TraceId.ToString()));
        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("SpanId", activity.SpanId.ToString()));
    }
}
