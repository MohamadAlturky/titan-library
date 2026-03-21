namespace Titan.Library.Common.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);

    Task<(List<string> Keys, long NextCursor)> ScanKeysAsync(
        long cursor,
        int count,
        CancellationToken ct = default
    );
}
