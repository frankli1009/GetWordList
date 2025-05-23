using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dictionary.Utilities
{
    public static class Extensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }

        public static IEnumerable<string> HasVowel(this IEnumerable<string> source)
        {
            Regex rgx = new Regex("[aeiouwy]");
            return source.Where(s => rgx.IsMatch(s));
        }

        public static IEnumerable<string> MatchExtraRegex(this IEnumerable<string> source, string extraRegex)
        {
            Regex rgx = new Regex(extraRegex);
            return source.Where(s => rgx.IsMatch(s));
        }

    }
}
