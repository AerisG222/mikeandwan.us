namespace Maw.Cache;

public class CacheResult<T>
{
    public bool ShouldUseResult { get; }
    public T Item { get; }

    public CacheResult(bool shouldUseResult, T item)
    {
        ShouldUseResult = shouldUseResult;
        Item = item;
    }
}
