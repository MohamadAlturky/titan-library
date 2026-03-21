using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Contracts.Cache;
using Titan.Library.Common.Caching;

namespace Titan.Library.Application.Cache;

public class CreateCacheRecordCommand : ICommand<CacheRecordDto>
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class CreateCacheRecordCommandHandler
    : ICommandHandler<CreateCacheRecordCommand, CacheRecordDto>
{
    private readonly ICacheService _cache;

    public CreateCacheRecordCommandHandler(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<Result<CacheRecordDto>> Handle(
        CreateCacheRecordCommand request,
        CancellationToken cancellationToken
    )
    {
        var record = new CacheRecordDto
        {
            Id = Guid.NewGuid().ToString(),
            Key = request.Key,
            Value = request.Value,
            CreatedAt = DateTime.UtcNow,
        };

        await _cache.SetAsync(request.Key, record);

        return Result<CacheRecordDto>.Success(
            record,
            ApplicationMessageKeys.CACHE_RECORD_CREATED_SUCCESSFULLY
        );
    }
}
