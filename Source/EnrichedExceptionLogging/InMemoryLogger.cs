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
        public IStructuredLogMessageAppender MessageAppender { get; }

        private readonly LoggerExternalScopeProvider _externalScopeProvider;
        public InMemoryLogger(ILoggingMessageQuee messageQuee, string categoryName, IStructuredLogMessageAppender messageAppender)
        {
            MessageQuee = messageQuee;
            CategoryName = categoryName;
            MessageAppender = messageAppender;
            _externalScopeProvider = new LoggerExternalScopeProvider();
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            object updatedState = state;
            if (state is IReadOnlyList<KeyValuePair<string, object>> kvpl)
            {
                if (kvpl.Last().Value != null)
                {
                    updatedState = MessageAppender.Append(kvpl, logLevel, CategoryName);
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
