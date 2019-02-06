using FluentSearch.Core;
using FluentSearch.Tests.Common;
using System.IO;

namespace FluentSearch.Tests.Intergration
{
    public class TestBase
    {
        public string Key = "Id";

        public string[] AnalyzedFields = new[] { "Name", "Email", "Mobile" };
        public string[] NonAnalyzedFields = new[] { "Id" };
        public string SearchTerm = "testdata";

        public string IndexPath = Directory.GetCurrentDirectory() + "/.search_indexes";


        public SearchContext<User> UserContext {
            get
            {
                return new SearchContext<User>(config =>
                {
                    config.Key = Key;
                    config.IndexPath = IndexPath;
                    config.AnalyzedFields = AnalyzedFields;
                    config.NonAnalyzedFields = NonAnalyzedFields;
                    config.ResultMapper = UserMapper.UserResultMapper;
                });
            }
        }

    }
}
