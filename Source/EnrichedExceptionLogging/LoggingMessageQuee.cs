using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace EnrichedExceptionLogging
{
    public struct LoggingMessage
    {
        public LogLevel LogLevel;

        public EventId EventId;

        public object State;

        public Exception Exception;

        public Func<object, Exception, string> Formatter;        
    }

    public interface ILoggingMessageQuee
    {
        void Enqueue(LoggingMessage item);
        LoggingMessage Dequeue();
        int Count { get; }
        void Clear();
    }

    public class LoggingMessageQuee : Queue<LoggingMessage>,ILoggingMessageQuee
    {}
}
