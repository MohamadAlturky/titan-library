using Titan.Library.Common.Caching;
using Titan.Library.Contracts.Messages;
using Titan.Library.Common.Caching;

namespace Titan.Library.Application.Messages.Caching;

public interface IMessageCacheValueResolver : ICacheValueResolver<string, MessageDto> { }
