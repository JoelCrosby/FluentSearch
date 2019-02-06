using System;
using System.Collections.Generic;

namespace FluentSearch.Core
{
    public class SearchContext<T>
    {

        #region Private Fields

        /// <summary>
        /// Main FluentIndex handle
        /// </summary>
        private readonly FluentIndex<T> _fluentIndex;

        /// <summary>
        /// Main Fluent Searcher Handle
        /// </summary>
        private readonly FluentSearcher<T> _fluentSearcher;

        /// <summary>
        /// Main search context configration container
        /// </summary>
        private readonly FluentConfig<T> _searchConfig;

        /// <summary>
        /// Wrapper for static default config values
        /// </summary>
        private readonly FluentDefaults<T> _fluentDefaults;

        #endregion

        #region Public Properties



        #endregion


        #region Constructor

        public SearchContext(Action<FluentConfig<T>> configure)
        {
            _fluentDefaults = new FluentDefaults<T>();

            var config = _fluentDefaults.DefaultConfig();
            configure(config);
            _searchConfig = config;
            

            _fluentIndex = new FluentIndex<T>(_searchConfig);
            _fluentSearcher = new FluentSearcher<T>(_searchConfig, _fluentIndex);
        }

        #endregion

        #region Public Methods


        /// <summary>
        /// Adds a single item to the lucene search index.
        /// </summary>
        /// <param name="sampleData"></param>
        public void Add(T item)
        {
            _fluentIndex.Add(new List<T> { item });
        }


        /// <summary>
        /// Adds or updates a list of items to the lucene search index.
        /// </summary>
        /// <param name="items"></param>
        public void Add(List<T> items)
        {
            _fluentIndex.Add(items);
        }

        /// <summary>
        /// Clears the entire search index.
        /// </summary>
        /// <returns></returns>
        public bool ClearLuceneIndex()
        {
            return _fluentIndex.Clear();
        }

        /// <summary>
        /// Get all indexed records.
        /// </summary>
        /// <returns></returns>
        public IList<T> GetAllIndexRecords()
        {
            return _fluentIndex.GetAllIndexRecords();
        }

        /// <summary>
        /// Searchs the lucene indexs.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public IList<T> Search(string input, string fieldName = "", int nPerPage = 30, int page = 0)
        {
            return _fluentSearcher.Search(input, fieldName, nPerPage, page);
        }


        /// <summary>
        /// Perform search on index. SearchDefault does not parse input query.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public IList<T> SearchDefault(string input, string fieldName = "", int nPerPage = 30, int page = 0)
        {
            return _fluentSearcher.SearchDefault(input, fieldName, nPerPage, page);
        }

        #endregion

    }
}