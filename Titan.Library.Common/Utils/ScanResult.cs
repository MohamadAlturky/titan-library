namespace Titan.Library.Common.Utils;

public class ScanResult<T>
{
    public List<T> Items { get; set; } = [];
    public long NextCursor { get; set; }
    public bool HasMore => NextCursor != 0;
}
