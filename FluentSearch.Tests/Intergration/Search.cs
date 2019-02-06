using Xunit;
using FluentSearch.Tests.Common;

namespace FluentSearch.Tests.Intergration
{
    public class Search : TestBase
    {

        [Fact]
        public void PerformSearch_With_Defaults()
        {

            var searchContext = UserContext;

            var users = Data.GetRandomUsers(10000);
            searchContext.Add(users);

            var results = searchContext.Search(SearchTerm);

            Assert.NotNull(results);
            Assert.True(results.Count >= 1);

            Assert.True(results[0].Id != null);
            Assert.True(results[0].Name != null);
            Assert.True(results[0].Email != null);
            Assert.True(results[0].Mobile != null);
        }

        [Fact]
        public void PerformSearch_With_CustomMapper()
        {

            var searchContext = UserContext;

            var users = Data.GetRandomUsers(10000);
            searchContext.Add(users);

            var results = searchContext.Search(SearchTerm);

            Assert.NotNull(results);
            Assert.True(results.Count >= 1);

            Assert.True(results[0].Id != null);
            Assert.True(results[0].Name != null);
            Assert.True(results[0].Email != null);
            Assert.True(results[0].Mobile != null);
        }
    }
}
