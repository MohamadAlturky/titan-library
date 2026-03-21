namespace Titan.Library.Api.Infrastructure.Observability;

public sealed class ObservabilityOptions
{
    public const string SectionName = "Observability";

    public string ServiceName { get; init; } = "Titan.Library";
    public string ServiceVersion { get; init; } = "1.0.0";
}
