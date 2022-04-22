namespace Maw.Domain.Search;

public class SearchResults<T>
{
    public IEnumerable<T> Results { get; set; } = new List<T>();
    public long TotalFound { get; set; }
    public long StartIndex { get; set; }
}
