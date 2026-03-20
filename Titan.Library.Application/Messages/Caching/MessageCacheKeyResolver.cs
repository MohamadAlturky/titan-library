namespace Titan.Library.Application.Messages.Caching;

public class MessageCacheKeyResolver : IMessageCacheKeyResolver
{
    public string Resolve(string lookup) => $"message:{lookup}";
}
