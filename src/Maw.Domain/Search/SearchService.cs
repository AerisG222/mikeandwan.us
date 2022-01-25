using System;
using System.Threading.Tasks;
using SolrNet;
using SolrNet.Commands.Parameters;
using Maw.Domain.Models.Search;

namespace Maw.Domain.Search;

public class SearchService
    : ISearchService
{
    readonly ISolrOperations<MultimediaCategory> _solr;

    public SearchService(ISolrOperations<MultimediaCategory> solr)
    {
        _solr = solr ?? throw new ArgumentNullException(nameof(solr));
    }

    public async Task<SearchResults<MultimediaCategory>> SearchAsync(string[] roles, string query, int start)
    {
        var opts = GetQueryOptions(roles, start);

        var solrResults = await _solr.QueryAsync(new SolrQuery(query), opts);

        return new SearchResults<MultimediaCategory>()
        {
            Results = solrResults.ToArray(),
            StartIndex = solrResults.Start,
            TotalFound = solrResults.NumFound
        };
    }

    static QueryOptions GetQueryOptions(string[] roles, int start)
    {
        var opts = new QueryOptions
        {
            RequestHandler = new RequestHandlerParameters("/maw-photos-query"),
            StartOrCursor = new StartOrCursor.Start(start),
            Rows = 24
        };

        opts.FilterQueries = new ISolrQuery[]
            {
                    new SolrQueryInList("allowed_roles", roles)
            };

        return opts;
    }
}
