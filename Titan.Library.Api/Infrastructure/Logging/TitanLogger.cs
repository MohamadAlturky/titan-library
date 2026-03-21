using Serilog.Context;
using Titan.Library.Common.Logging;

namespace Titan.Library.Api.Infrastructure.Logging;

public sealed class TitanLogger<T> : ITitanLogger<T>
{
    private readonly ILogger<T> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICorrelationIdProvider _correlationIdProvider;

    public TitanLogger(
        ILogger<T> logger,
        IHttpContextAccessor httpContextAccessor,
        ICorrelationIdProvider correlationIdProvider
    )
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _correlationIdProvider = correlationIdProvider;
    }

    public void LogInformation(string messageTemplate, params object?[] args)
    {
        using var ctx = PushContext();
        _logger.LogInformation(messageTemplate, args);
    }

    public void LogWarning(string messageTemplate, params object?[] args)
    {
        using var ctx = PushContext();
        _logger.LogWarning(messageTemplate, args);
    }

    public void LogError(Exception? exception, string messageTemplate, params object?[] args)
    {
        using var ctx = PushContext();
        _logger.LogError(exception, messageTemplate, args);
    }

    public void LogCritical(Exception? exception, string messageTemplate, params object?[] args)
    {
        using var ctx = PushContext();
        _logger.LogCritical(exception, messageTemplate, args);
    }

    public void LogDebug(string messageTemplate, params object?[] args)
    {
        using var ctx = PushContext();
        _logger.LogDebug(messageTemplate, args);
    }

    private IDisposable PushContext()
    {
        var http = _httpContextAccessor.HttpContext;

        var correlationId = _correlationIdProvider.CorrelationId;
        var userId = http?.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value ?? string.Empty;
        var requestPath = http?.Request.Path.ToString() ?? string.Empty;
        var requestMethod = http?.Request.Method ?? string.Empty;

        return new CompositeDisposable(
            LogContext.PushProperty("CorrelationId", correlationId),
            LogContext.PushProperty("UserId", userId),
            LogContext.PushProperty("RequestPath", requestPath),
            LogContext.PushProperty("RequestMethod", requestMethod),
            LogContext.PushProperty("MachineName", Environment.MachineName)
        );
    }

    private sealed class CompositeDisposable : IDisposable
    {
        private readonly IDisposable[] _disposables;

        public CompositeDisposable(params IDisposable[] disposables)
        {
            _disposables = disposables;
        }

        public void Dispose()
        {
            for (var i = _disposables.Length - 1; i >= 0; i--)
                _disposables[i].Dispose();
        }
    }
}
