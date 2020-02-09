using System.Threading.Tasks;


namespace Maw.Domain.Search
{
    public interface ISearchService
    {
        Task<SearchResults<MultimediaCategory>> SearchAsync(bool allowPrivate, string query, int start);
    }
}
