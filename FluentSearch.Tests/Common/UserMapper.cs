using FluentSearch.Core;
using System.Collections.Generic;

namespace FluentSearch.Tests.Common
{
    public class UserMapper
    {

        public static IList<User> UserResultMapper(IEnumerable<SearchResult> docs)
        {
            var res = new List<User>();

            foreach (var result in docs)
            {
                var user = new User
                {
                    Name = result.Fields["Name"],
                    Email = result.Fields["Email"],
                    Mobile = result.Fields["Mobile"],
                    Id = result.Fields["Id"],
                };

                res.Add(user);
            }

            return res;
        }

    }
}
