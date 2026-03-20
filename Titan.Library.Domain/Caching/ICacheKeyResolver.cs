namespace Titan.Library.Domain.Caching;

public interface ICacheKeyResolver<in TLookup>
{
    string Resolve(TLookup lookup);
}
