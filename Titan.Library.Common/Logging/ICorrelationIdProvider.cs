namespace Titan.Library.Common.Logging;

public interface ICorrelationIdProvider
{
    string CorrelationId { get; }
    void Set(string correlationId);
}
