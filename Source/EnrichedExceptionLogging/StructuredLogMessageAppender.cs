using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;

namespace EnrichedExceptionLogging
{
    public interface IStructuredLogMessageAppender
    {
        FormattedLogValues Append(IReadOnlyList<KeyValuePair<string, object>> message, LogLevel originalLevel, string categoryName);
    }

    public class StructuredLogMessageAppender : IStructuredLogMessageAppender
    {
        public FormattedLogValues Append(IReadOnlyList<KeyValuePair<string, object>> message, LogLevel originalLevel, string categoryName)
        {
            var newMessage = "{OriginalSourceContext} {OriginalLogLevel} {OriginalTimeStamp} " + message.Last().Value;
            var na = new object[] {categoryName, originalLevel, DateTime.Now.ToString("O")}
                .Concat(message.Take(message.Count - 1).Select(kvp => kvp.Value)).ToArray();
            return new FormattedLogValues(newMessage, na);
        }
    }  
}