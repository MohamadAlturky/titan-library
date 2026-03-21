namespace Titan.Library.Common.Caching;

public interface ICacheValueResolver<in TLookup, TDto> where TDto : class
{
    Task<TDto?> GetAsync(TLookup lookup, CancellationToken ct = default);
    Task SetAsync(TLookup lookup, TDto value, CancellationToken ct = default);
    Task RemoveAsync(TLookup lookup, CancellationToken ct = default);
}
