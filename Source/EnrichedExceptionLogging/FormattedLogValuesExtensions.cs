using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;

namespace EnrichedExceptionLogging
{
    public static class FormattedLogValuesExtensions
    {
        public static FormattedLogValues AddTimeStamp(this FormattedLogValues flv)
                =>  AppendTuple(flv, "TimeStamp", DateTime.Now.ToString("O"));
  
        public static FormattedLogValues AddLogLevel(this FormattedLogValues flv, LogLevel level) =>
            AppendTuple(flv, "OriginalLogLevel", level);

        public static FormattedLogValues AddCategoryName(this FormattedLogValues flv, string categoryName) =>
            AppendTuple(flv, "CategoryName", categoryName);

        private static FormattedLogValues AppendTuple(FormattedLogValues flv, string key, object value)
        {
            var deomposed = DecomposedFormattedLogValues.Decompose(flv);
            return new FormattedLogValues("{"+key+"} " + deomposed.OriginalMessage, new object[] { value }.Concat(deomposed.Args).ToArray());
        }
    }

    public class DecomposedFormattedLogValues
    {
        public string OriginalMessage { get; }
        public object[] Args { get; }


        private DecomposedFormattedLogValues(string originalMessage, object[] args)
        {
            OriginalMessage = originalMessage;
            Args = args;
        }

        public static DecomposedFormattedLogValues Decompose(FormattedLogValues flv)
        {
            var args = flv.Select(kvp => kvp.Value).Take(flv.Count - 1);
            return new DecomposedFormattedLogValues(flv.Last().Value.ToString(), args.ToArray());
        }
    }
}