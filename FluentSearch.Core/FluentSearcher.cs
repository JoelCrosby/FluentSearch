using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using System.Collections.Generic;
using System.Linq;

namespace FluentSearch.Core
{
    public class FluentSearcher<T>
    {


        /// <summary>
        /// Main search context configration container
        /// </summary>
        private readonly FluentConfig<T> _searchConfig;


        /// <summary>
        /// Main FluentIndex handle
        /// </summary>
        private readonly FluentIndex<T> _fluentIndex;


        protected internal FluentSearcher(FluentConfig<T> searchConfig, FluentIndex<T> fluentIndex)
        {
            _searchConfig = searchConfig;
            _fluentIndex = fluentIndex;
        }


        /// <summary>
        /// Searchs the underlying lucene indexs and returns a list of results.
        /// </summary>
        /// <param name="input">The search query string</param>
        /// <param name="fieldName">Optiional field to search by</param>
        /// <param name="nPerPage">Number of results returned per page</param>
        /// <param name="page">Result Page to return</param>
        /// <returns></returns>
        protected internal IList<T> Search(string input, string fieldName = "", int nPerPage = 30, int page = 0)
        {
            if (string.IsNullOrEmpty(input)) return new List<T>();

            var terms = input.Trim().Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return _search(input, fieldName, nPerPage, page);
        }


        /// <summary>
        /// Perform search on index. SearchDefault does not parse input query.
        /// </summary>
        /// <param name="input">The search query string</param>
        /// <param name="fieldName">Optiional field to search by</param>
        /// <param name="nPerPage">Number of results returned per page</param>
        /// <param name="page">Result Page to return</param>
        /// <returns></returns>
        protected internal IList<T> SearchDefault(string input, string fieldName = "", int nPerPage = 30, int page = 0)
        {
            return string.IsNullOrEmpty(input) ? new List<T>() : _search(input, fieldName, nPerPage, page);
        }


        /// <summary>
        /// The main lucene search method.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="searchField"></param>
        /// <returns></returns>
        private IList<T> _search(string searchQuery, string searchField = "", int hitlimit = 30, int page = 1)
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<T>();

            // set up lucene searcher
            using (IndexReader reader = DirectoryReader.Open(_fluentIndex.Directory))
            {
                var searcher = new IndexSearcher(reader);
                var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(LuceneVersion.LUCENE_48, searchField, analyzer);
                    var query = _parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hitlimit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Dispose();
                    return results;
                }
                // search by multiple fields (ordered by RELEVANCE)
                else
                {
                    var parser = new MultiFieldQueryParser(
                        LuceneVersion.LUCENE_48,
                        _searchConfig.AnalyzedFields.Concat(_searchConfig.NonAnalyzedFields).ToArray(),
                        analyzer
                    );

                    var query = _parseQuery(searchQuery, parser);
                    var skip = hitlimit * page;
                    var hits = searcher.Search(query, null, hitlimit + skip, Sort.RELEVANCE).ScoreDocs;
                    var pagedHits = hits.Skip(skip);
                    var results = _mapLuceneToDataList(pagedHits, searcher);
                    analyzer.Dispose();
                    return results;
                }
            }
        }


        /// <summary>
        /// Parses the input search query string into a Lucene Query object.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        private Query _parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }


        /// <summary>
        /// Maps document results to data objects.
        /// </summary>
        /// <param name="hits"></param>
        /// <returns></returns>
        private IList<T> _mapLuceneToDataList(IEnumerable<Document> hits)
        {

            var fields = new List<SearchResult>();

            foreach (var hit in hits)
            {

                fields.Add(new SearchResult
                {
                    Fields = hit.Fields.ToDictionary(t => t.Name, t => t.GetStringValue())
                });
            }

            return _searchConfig.ResultMapper(fields);

        }

        /// <summary>
        /// Maps score doc results to data objects.
        /// </summary>
        /// <param name="hits"></param>
        /// <param name="searcher"></param>
        /// <returns></returns>
        private IList<T> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {

            var fields = new List<SearchResult>();

            foreach (var hit in hits)
            {

                fields.Add(new SearchResult
                {
                    Fields = searcher.Doc(hit.Doc).Fields.ToDictionary(t => t.Name, t => t.GetStringValue())
                });
            }

            return _searchConfig.ResultMapper(fields);
        }

    }
}
