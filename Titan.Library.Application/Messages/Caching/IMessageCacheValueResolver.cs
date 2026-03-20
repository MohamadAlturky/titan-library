using Titan.Library.Contracts.Messages;
using Titan.Library.Domain.Caching;

namespace Titan.Library.Application.Messages.Caching;

public interface IMessageCacheValueResolver : ICacheValueResolver<string, MessageDto> { }
