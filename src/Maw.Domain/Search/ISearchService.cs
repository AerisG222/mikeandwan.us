using System.Threading.Tasks;

namespace Maw.Domain.Search;

public interface ISearchService
{
    Task<SearchResults<MultimediaCategory>> SearchAsync(string[] roles, string query, int start);
}
