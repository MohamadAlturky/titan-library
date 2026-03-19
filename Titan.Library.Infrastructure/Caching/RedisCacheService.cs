using System.Text.Json;
using StackExchange.Redis;
using Titan.Library.Domain.Caching;

namespace Titan.Library.Infrastructure.Caching;

public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var value = await _db.StringGetAsync(key);

        return value.HasValue ? JsonSerializer.Deserialize<T>((string)value!) : default;
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiry = null,
        CancellationToken ct = default
    )
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiry);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default) =>
        await _db.KeyDeleteAsync(key);

    public async Task<(List<string> Keys, long NextCursor)> ScanKeysAsync(
        long cursor,
        int count,
        CancellationToken ct = default
    )
    {
        var result = await _db.ExecuteAsync("SCAN", cursor.ToString(), "COUNT", count.ToString());
        var innerResult = (RedisResult[])result!;

        var nextCursor = (long)innerResult[0];
        var keys = ((string[])innerResult[1]!).ToList();

        return (keys, nextCursor);
    }
}
