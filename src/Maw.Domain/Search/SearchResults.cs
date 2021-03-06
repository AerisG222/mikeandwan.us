using System.Collections.Generic;


namespace Maw.Domain.Search
{
    public class SearchResults<T> {
        public IEnumerable<T> Results { get; set; }
        public int TotalFound { get; set; }
        public int StartIndex { get; set; }
    }
}
