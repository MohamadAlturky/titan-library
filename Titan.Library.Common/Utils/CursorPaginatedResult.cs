namespace Titan.Library.Common.Utils;

public class CursorPaginatedResult<T>
{
    public T[] Items { get; set; } = [];
    public int? NextCursor { get; set; }
    public bool HasMore { get; set; }

    public CursorPaginatedResult(List<T> items, bool hasMore, int? nextCursor)
    {
        Items = items.ToArray();
        HasMore = hasMore;
        NextCursor = nextCursor;
    }
}
