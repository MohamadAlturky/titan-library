using Titan.Library.Common.Logging;

namespace Titan.Library.Api.Infrastructure;

public sealed class CorrelationIdProvider : ICorrelationIdProvider
{
    private string _correlationId = Guid.NewGuid().ToString();

    public string CorrelationId => _correlationId;

}
