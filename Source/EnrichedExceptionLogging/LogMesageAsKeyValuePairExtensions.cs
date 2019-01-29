using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace EnrichedExceptionLogging
{
    public static class LogMesageAsKeyValuePairExtensions
    {
        public static IReadOnlyList<KeyValuePair<string, object>> AddTimeStamp(this IReadOnlyList<KeyValuePair<string, object>> flv)
            => AppendTuple(flv, "OriginalTimeStamp", DateTime.Now.ToString("O"));

        public static IReadOnlyList<KeyValuePair<string, object>> AddLogLevel(this IReadOnlyList<KeyValuePair<string, object>> flv, LogLevel level) =>
            AppendTuple(flv, "OriginalLogLevel", level);

        public static IReadOnlyList<KeyValuePair<string, object>> AddCategoryName(this IReadOnlyList<KeyValuePair<string, object>> flv, string categoryName) =>
            AppendTuple(flv, "OriginalSourceContext", categoryName);

        private static IReadOnlyList<KeyValuePair<string, object>> AppendTuple(
            IReadOnlyList<KeyValuePair<string, object>> flv, string key,
            object value)
        {
            var newMessage = new KeyValuePair<string, object>(flv.Last().Key, "{" + key + "} " + flv.Last().Value);

            return new List<KeyValuePair<string, object>>(new[] {new KeyValuePair<string, object>(key, value)}).
                Concat(flv.Take(flv.Count-1)).Concat(new [] { newMessage }).ToList();
        }

        public static IReadOnlyList<KeyValuePair<string, object>> AppendAll(this 
            IReadOnlyList<KeyValuePair<string, object>> flv, IList<KeyValuePair<string,object>> newVals)
        {
            var newMessage = newVals.Select(v => v.Value).Aggregate(flv.Last().Value.ToString(), (current, val) => "{" + val + "} " + current);
            return newVals.Concat(flv.Take(flv.Count - 1)).Concat(new [] {new KeyValuePair<string, object>(flv.Last().Key, newMessage)}).ToList();
        }
    }

  
}