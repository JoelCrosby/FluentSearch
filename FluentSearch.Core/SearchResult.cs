using System.Collections.Generic;

namespace FluentSearch.Core
{
    /// <summary>
    /// Generic search result wrapper.
    /// </summary>
    public class SearchResult
    {
        public Dictionary<string, string> Fields { get; set; }
    }
}
