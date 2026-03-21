using System.Diagnostics;
using MediatR;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Titan.Library.Api.Infrastructure.Observability;

public static class ObservabilityExtensions
{
    public static readonly ActivitySource TitanActivitySource = new("Titan.Library", "1.0.0");

    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var opts =
            configuration.GetSection(ObservabilityOptions.SectionName).Get<ObservabilityOptions>()
            ?? new ObservabilityOptions();

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService(serviceName: opts.ServiceName, serviceVersion: opts.ServiceVersion)
            .AddTelemetrySdk()
            .AddEnvironmentVariableDetector();
            

        services
            .AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(resourceBuilder)
                    .AddSource(TitanActivitySource.Name)
                    .AddAspNetCoreInstrumentation(o =>
                    {
                        o.EnrichWithHttpRequest = (activity, request) =>
                        {
                            var correlationId = request
                                .Headers["X-Correlation-ID"]
                                .FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(correlationId))
                                activity.SetTag("correlation.id", correlationId);
                        };
                        o.RecordException = true;
                    })
                    .AddHttpClientInstrumentation()
                    .AddSource("Npgsql"); // Npgsql v8+ has built-in OTel tracing

                if (IsDevelopment())
                    tracing.AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddMeter(LibraryMetrics.MeterName)
                    // ASP.NET Core built-in meters (explicit — AddAspNetCoreInstrumentation does not include Kestrel)
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Routing")
                    .AddMeter("Microsoft.AspNetCore.Diagnostics")
                    .AddMeter("Microsoft.AspNetCore.RateLimiting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation();

                if (IsDevelopment())
                    metrics.AddConsoleExporter();

                metrics.AddPrometheusExporter();
            });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MetricsBehavior<,>));

        return services;
    }

    private static bool IsDevelopment() =>
        (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "").Equals(
            "Development",
            StringComparison.OrdinalIgnoreCase
        );
}
