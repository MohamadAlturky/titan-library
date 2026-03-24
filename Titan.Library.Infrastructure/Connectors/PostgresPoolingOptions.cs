namespace Titan.Library.Infrastructure.Connectors;

public sealed class PostgresPoolingOptions
{
    public const string SectionName = "ConnectionPooling";

    public int MinPoolSize { get; init; } = 1;
    public int MaxPoolSize { get; init; } = 20;

    /// <summary>Seconds a connection may remain idle before being pruned.</summary>
    public int ConnectionIdleLifetimeSeconds { get; init; } = 300;

    /// <summary>Seconds to wait for a pooled connection before throwing a timeout exception.</summary>
    public int ConnectionTimeoutSeconds { get; init; } = 15;
}
