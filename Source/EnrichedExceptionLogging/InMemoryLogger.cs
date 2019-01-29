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
            if (state is FormattedLogValues)
            {
                updatedState = (state as FormattedLogValues).AddCategoryName(CategoryName).AddLogLevel(logLevel).AddTimeStamp();
            }
            else if (state is IReadOnlyList<KeyValuePair<string, object>>)
            {
                var kvpl = state as IReadOnlyList<KeyValuePair<string, object>>;
                if (kvpl.Last().Value != null)
                {
                    var message = kvpl.Last().Value.ToString();
                    var args = kvpl.Take(kvpl.Count - 1).Select(kvp => kvp.Value).ToArray();
                    var flv = new FormattedLogValues(message, args);
                    updatedState = flv.AddCategoryName(CategoryName).AddLogLevel(logLevel).AddTimeStamp();
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
