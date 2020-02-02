using System.Threading.Tasks;
using SolrNet;


namespace Maw.Domain.Search
{
    public interface ISearchService
    {
        // ideally we would hide solrqueryresults but is unlikely we will swap this out anytime soon,
        // so deal with the leaky abstraction
        Task<SolrQueryResults<MultimediaCategory>> SearchAsync(bool allowPrivate, string query, int start);
    }
}
