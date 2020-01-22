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


        public Task<SolrQueryResults<MultimediaCategory>> SearchAsync(bool allowPrivate, string query)
        {
            var opts = GetQueryOptions(allowPrivate);

            return _solr.QueryAsync(new SolrQuery(query), opts);
        }


        static QueryOptions GetQueryOptions(bool allowPrivate)
        {
            var opts = new QueryOptions
            {
                RequestHandler = new RequestHandlerParameters("/maw-photos-query")
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
