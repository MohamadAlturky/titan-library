namespace Titan.Library.Common.Logging;

public interface ICorrelationIdProvider
{
    string CorrelationId { get; }
}
