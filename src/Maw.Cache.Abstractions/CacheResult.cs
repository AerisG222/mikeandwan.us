namespace Maw.Cache.Abstractions;

public class CacheResult<T>
{
    public bool ShouldUseResult { get; }
    public T? Item { get; }
    public T Value
    {
        get =>  Item ?? throw new InvalidOperationException();
    }

    public CacheResult(bool shouldUseResult, T? item)
    {
        ShouldUseResult = shouldUseResult;
        Item = item;
    }
}
