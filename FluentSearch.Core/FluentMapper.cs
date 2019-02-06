using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FluentSearch.Core
{

    /// <summary>
    /// Container for search result to Object mapping Methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FluentMapper<T>
    {

        /// <summary>
        /// Maps a list of SearchResults to a list of the given type.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static IList<T> ReflectMap(List<SearchResult> results)
        {
            return results.Select(item => DictionaryToObject<T>(item.Fields)).ToList();
        }


        /// <summary>
        /// Maps a flat dictionary to a generic type.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        private static K DictionaryToObject<K>(IDictionary<string, string> dict)
        {
            // Allows creating an instance of a generic type.
            // The type nees to have a parameterless contructor although this is enforced at compile time.
            var t = Activator.CreateInstance<K>();
            PropertyInfo[] properties = t.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                    continue;

                KeyValuePair<string, string> item = dict.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));

                // Find which property type (int, string, double? etc) the CURRENT property is...
                Type tPropertyType = t.GetType().GetProperty(property.Name).PropertyType;

                // Fix nullables...
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;

                // ...and change the type
                object newA = Convert.ChangeType(item.Value, newT);
                t.GetType().GetProperty(property.Name).SetValue(t, newA, null);
            }
            return t;
        }

    }
}
