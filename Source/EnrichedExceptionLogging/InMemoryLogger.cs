using System;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace EnrichedExceptionLogging
{
    public class InMemoryLogger : ILogger
    {
        public ILoggingMessageQuee MessageQuee { get; }

        private readonly LoggerExternalScopeProvider _externalScopeProvider;
        public InMemoryLogger(ILoggingMessageQuee messageQuee)
        {
            MessageQuee = messageQuee;
            _externalScopeProvider = new LoggerExternalScopeProvider();
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            MessageQuee.Enqueue(new LoggingMessage
            {
                EventId = eventId,
                LogLevel = logLevel,
                Message = formatter(state,exception)
            });
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) =>
            _externalScopeProvider.Push(state);
    }
}
