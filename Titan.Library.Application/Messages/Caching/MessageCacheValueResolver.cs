using Titan.Library.Contracts.Messages;
using Titan.Library.Common.Caching;

namespace Titan.Library.Application.Messages.Caching;

public class MessageCacheValueResolver : IMessageCacheValueResolver
{
    private readonly ICacheService _cache;
    private readonly IMessageCacheKeyResolver _keyResolver;

    public MessageCacheValueResolver(ICacheService cache, IMessageCacheKeyResolver keyResolver)
    {
        _cache = cache;
        _keyResolver = keyResolver;
    }

    public Task<MessageDto?> GetAsync(string lookup, CancellationToken ct = default) =>
        _cache.GetAsync<MessageDto>(_keyResolver.Resolve(lookup), ct);

    public Task SetAsync(string lookup, MessageDto value, CancellationToken ct = default) =>
        _cache.SetAsync(_keyResolver.Resolve(lookup), value, null, ct);

    public Task RemoveAsync(string lookup, CancellationToken ct = default) =>
        _cache.RemoveAsync(_keyResolver.Resolve(lookup), ct);
}
