using Titan.Library.Common.Cqrs;
using Titan.Library.Common.Results;
using Titan.Library.Common.Utils;
using Titan.Library.Contracts.Cache;
using Titan.Library.Domain.Caching;

namespace Titan.Library.Application.Cache;

public class GetCacheRecordsQuery : IQuery<ScanResult<CacheRecordDto>>
{
    public long Cursor { get; set; } = 0;
    public int Count { get; set; } = 10;
}

public class GetCacheRecordsQueryHandler
    : IQueryHandler<GetCacheRecordsQuery, ScanResult<CacheRecordDto>>
{
    private readonly ICacheService _cache;

    public GetCacheRecordsQueryHandler(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<Result<ScanResult<CacheRecordDto>>> Handle(
        GetCacheRecordsQuery request,
        CancellationToken cancellationToken
    )
    {
        var count = request.Count < 1 ? 10 : request.Count;

        var (keys, nextCursor) = await _cache.ScanKeysAsync(
            request.Cursor,
            count,
            cancellationToken
        );

        var items = new List<CacheRecordDto>();
        foreach (var key in keys)
        {
            var record = await _cache.GetAsync<CacheRecordDto>(key, cancellationToken);
            if (record is not null)
                items.Add(record);
        }

        var result = new ScanResult<CacheRecordDto> { Items = items, NextCursor = nextCursor };

        return Result<ScanResult<CacheRecordDto>>.Success(
            result,
            ApplicationMessageKeys.CACHE_RECORDS_RETRIEVED_SUCCESSFULLY
        );
    }
}
