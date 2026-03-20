namespace Titan.Library.Common.EndPoints;

public interface IApiResponseResolver
{
    Task<string> ResolveAsync(string key, CancellationToken cancellationToken = default);
}
