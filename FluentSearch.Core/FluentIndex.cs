using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FluentSearch.Core
{
    public class FluentIndex<T>
    {


        /// <summary>
        /// Main search context configration container
        /// </summary>
        private readonly FluentConfig<T> _searchConfig;

        /// <summary>
        /// The Lucene directory instance
        /// </summary>
        private FSDirectory _directoryTemp;

        /// <summary>
        /// An instance of Lucene.Net class FSDirectory, and will be used by all of the search methods to access search index.
        /// </summary>
        private FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_searchConfig.IndexPath));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_searchConfig.IndexPath, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }

        /// <summary>
        /// An instance of Lucene.Net class FSDirectory, and will be used by all of the search methods to access search index.
        /// </summary>
        protected internal FSDirectory Directory { get => _directory; }


        protected internal FluentIndex(FluentConfig<T> searchConfig)
        {
            _searchConfig = searchConfig;
        }


        /// <summary>
        /// Get all indexed records.
        /// </summary>
        /// <returns></returns>
        protected internal IList<T> GetAllIndexRecords()
        {
            // validate search index
            if (!System.IO.Directory.EnumerateFiles(_searchConfig.IndexPath).Any()) return new List<T>();

            // set up lucene searcher
            using (IndexReader reader = DirectoryReader.Open(_directory))
            {
                var searcher = new IndexSearcher(reader);
                var docs = new List<Document>();
                // TODO: Implement get all index records
                reader.Dispose();
                return new List<T>();
            }
        }


        /// <summary>
        /// Adds or updates a list of items to the lucene search index.
        /// </summary>
        /// <param name="items"></param>
        protected internal void Add(IEnumerable<T> items)
        {

            // init lucene
            var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            var idc = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
            using (var writer = new IndexWriter(_directory, idc))
            {
                // add data to lucene search index (replaces older entry if any)
                foreach (var item in items) _addToLuceneIndex(item, writer);

                // close handles
                analyzer.Dispose();
                writer.Dispose();
            }
        }

        /// <summary>
        /// Clears the entire search index.
        /// </summary>
        /// <returns></returns>
        protected internal bool Clear()
        {
            try
            {
                var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
                var idc = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
                using (var writer = new IndexWriter(_directory, idc))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Dispose();
                    writer.Dispose();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Removes a single record from Lucene search index by record's Id field.
        /// </summary>
        /// <param name="record_id"></param>
        protected internal void Remove(string record_id)
        {
            // init lucene
            var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            var idc = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
            using (var writer = new IndexWriter(_directory, idc))
            {
                // remove older index entry
                var searchQuery = new TermQuery(new Term("Id", record_id));
                writer.DeleteDocuments(searchQuery);

                // close handles
                analyzer.Dispose();
                writer.Dispose();
            }
        }


        /// <summary>
        /// Creates a single search index entry based on the SearchItem Type.
        /// </summary>
        /// <param name="searchItem"></param>
        /// <param name="writer"></param>
        private void _addToLuceneIndex(T searchItem, IndexWriter writer)
        {

            // remove older index entry
            var objectKey = searchItem.GetType().GetProperty(_searchConfig.Key).GetValue(searchItem, null);
            if (objectKey == null) return;

            var objectKeyString = objectKey.ToString();
            var searchQuery = new TermQuery(
                new Term(_searchConfig.Key, objectKeyString)
            );

            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

            // None Analysed Fields
            foreach (var field in _searchConfig.NonAnalyzedFields)
            {
                var fieldToAdd = searchItem.GetType().GetProperty(field).GetValue(searchItem, null).ToString();

                if (!string.IsNullOrWhiteSpace(fieldToAdd))
                    doc.Add(new StringField(field, fieldToAdd, Field.Store.YES));
            }

            // Analysed
            foreach (var field in _searchConfig.AnalyzedFields)
            {
                var fieldToAdd = searchItem.GetType().GetProperty(field).GetValue(searchItem, null).ToString();

                if (!string.IsNullOrWhiteSpace(fieldToAdd))
                    doc.Add(new StringField(field, fieldToAdd, Field.Store.YES));
            }

            // add entry to index
            writer.AddDocument(doc);
        }

    }
}
