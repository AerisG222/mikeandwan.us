namespace MawApi.ViewModels;

public class ApiCollectionResult<T>
{
    public List<T> Items { get; }
    public long Count => Items.Count;

    public ApiCollectionResult(List<T> items)
    {
        Items = items;
    }
}
