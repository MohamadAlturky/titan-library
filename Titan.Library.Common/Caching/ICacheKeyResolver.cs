namespace Titan.Library.Common.Caching;

public interface ICacheKeyResolver<in TLookup>
{
    string Resolve(TLookup lookup);
}
