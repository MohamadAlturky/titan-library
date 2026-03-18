namespace Titan.Library.Common.Utils;

public class PaginatedResult<T>
{
    public T[] Items { get; set; } = [];
    public int TotalCount { get; set; } = 0;
    public int TotalPages { get; set; } = 0;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public PaginatedResult(List<T> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToArray();
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        Page = page;
        PageSize = pageSize;
    }
}
