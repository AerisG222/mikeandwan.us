using System.Threading.Tasks;
using Maw.Domain.Models.Search;

namespace Maw.Domain.Search;

public interface ISearchService
{
    Task<SearchResults<MultimediaCategory>> SearchAsync(string[] roles, string query, int start);
}
