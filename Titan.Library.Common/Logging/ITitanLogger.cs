namespace Titan.Library.Common.Logging;

public interface ITitanLogger<T>
{
    void LogInformation(string messageTemplate, params object?[] args);
    void LogWarning(string messageTemplate, params object?[] args);
    void LogError(Exception? exception, string messageTemplate, params object?[] args);
    void LogCritical(Exception? exception, string messageTemplate, params object?[] args);
    void LogDebug(string messageTemplate, params object?[] args);
}
