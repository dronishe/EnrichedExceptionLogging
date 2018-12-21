using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace EnrichedExceptionLogging
{
    public class LoggingMessage<TState>
    {
        public LogLevel LogLevel;

        public EventId EventId;

        public string Message;


        //EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter
        public TState State { get; set; }
    }

    public interface ILoggingMessageQuee<TState>
    {
        void Enqueue(LoggingMessage<TState> item);
        LoggingMessage<TState> Dequeue();
        int Count { get; }
        void Clear();
    }

    public class LoggingMessageQuee<TState> : Queue<LoggingMessage<TState>>, ILoggingMessageQuee<TState>
    { }
}
