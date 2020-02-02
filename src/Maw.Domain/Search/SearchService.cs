using System;
using System.Threading.Tasks;
using SolrNet;
using SolrNet.Commands.Parameters;


namespace Maw.Domain.Search
{
    public class SearchService
        : ISearchService
    {
        readonly ISolrOperations<MultimediaCategory> _solr;


        public SearchService(ISolrOperations<MultimediaCategory> solr)
        {
            _solr = solr ?? throw new ArgumentNullException(nameof(solr));
        }


        public async Task<SearchResults<MultimediaCategory>> SearchAsync(bool allowPrivate, string query, int start)
        {
            var opts = GetQueryOptions(allowPrivate, start);

            var solrResults = await _solr.QueryAsync(new SolrQuery(query), opts).ConfigureAwait(false);

            return new SearchResults<MultimediaCategory>() {
                Results = solrResults.ToArray(),
                StartIndex = solrResults.Start,
                TotalFound = solrResults.NumFound
            };
        }


        static QueryOptions GetQueryOptions(bool allowPrivate, int start)
        {
            var opts = new QueryOptions
            {
                RequestHandler = new RequestHandlerParameters("/maw-photos-query"),
                StartOrCursor = new StartOrCursor.Start(start),
                Rows = 24
            };

            if(!allowPrivate)
            {
                opts.FilterQueries = new ISolrQuery[]
                    {
                        new SolrQueryByField("is_private", "false")
                    };
            };

            return opts;
        }
    }
}
