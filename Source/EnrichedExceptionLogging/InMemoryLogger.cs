using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;

namespace EnrichedExceptionLogging
{
    public class InMemoryLogger : ILogger
    {
        public ILoggingMessageQuee MessageQuee { get; }
        public string CategoryName { get; }

        private readonly LoggerExternalScopeProvider _externalScopeProvider;
        public InMemoryLogger(ILoggingMessageQuee messageQuee, string categoryName)
        {
            MessageQuee = messageQuee;
            CategoryName = categoryName;
             _externalScopeProvider = new LoggerExternalScopeProvider();
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            object updatedState = state;
            if (state is IReadOnlyList<KeyValuePair<string, object>> kvpl)
            {
                if (kvpl.Last().Value != null)
                {
                    var updatedList = kvpl.AddCategoryName(CategoryName).AddLogLevel(logLevel).AddTimeStamp();
                    var message = updatedList.Last().Value.ToString();
                    var args = updatedList.Take(updatedList.Count - 1).Select(kvp => kvp.Value).ToArray();
                    updatedState = new FormattedLogValues(message, args);
                }
            }

            MessageQuee.Enqueue(new LoggingMessage
            {
                EventId = eventId,
                LogLevel = logLevel,
                State = updatedState,
                Exception = exception,
                Formatter = (s, e) => formatter(state,exception)
                
            });
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) =>
            _externalScopeProvider.Push(state);
    }
}
