using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;

namespace EnrichedExceptionLogging
{
    public class InMemoryLogger : ILogger
    {
        public ILoggingMessageQuee MessageQuee { get; }
        public string CategoryName { get; }

        private readonly LoggerExternalScopeProvider _externalScopeProvider;
        public InMemoryLogger(ILoggingMessageQuee messageQuee)
        {
            MessageQuee = messageQuee;
           
            _externalScopeProvider = new LoggerExternalScopeProvider();
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            object updatedState = state;
            if (state is FormattedLogValues)
            {
                updatedState = (state as FormattedLogValues).AddLogLevel(logLevel).AddTimeStamp();


            }
            //var qq = new List<KeyValuePair<string, object>>(
            //    updatedState as IReadOnlyList<KeyValuePair<string, object>>);

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
