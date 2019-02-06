using System.Collections.Generic;
using System.Linq;

namespace FluentSearch.Core
{
    /// <summary>
    /// Container object for FluentSearch context specific options.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FluentConfig<T>
    {

        /// <summary>
        /// Name of the field used by FluentSearch to uniquly identify index entries.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// List of model property names to add to the index as analyzed fields.
        /// </summary>
        public IEnumerable<string> AnalyzedFields { get; set; }

        /// <summary>
        /// List of model property names to add to the index as non analyzed fields.
        /// </summary>
        public IEnumerable<string> NonAnalyzedFields { get; set; }

        /// <summary>
        ///  The relative path to use as the index store.
        /// </summary>
        public string IndexPath { get; set; }

        /// <summary>
        /// Delegate method used for maping search result items back to model items.
        /// </summary>
        public ResutMapper ResultMapper { get; set; }


        /// <summary>
        /// Returns an array containing both Analyzed Fields and Non Analyzed Fields.
        /// </summary>
        public IEnumerable<string> AllFields { get => AnalyzedFields.Concat(NonAnalyzedFields); }


        #region Delegates

        public delegate IList<T> ResutMapper(List<SearchResult> results);

        #endregion

    }
}
