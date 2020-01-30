using Microsoft.Extensions.Logging;

namespace EnrichedExceptionLogging
{
    public class InMemoryLogginProvider : ILoggerProvider
    {
        public ILoggingMessageQuee MessageQuee { get; }
        public IStructuredLogMessageAppender MessageAppender { get; }

        public void Dispose()
        { }

        public InMemoryLogginProvider(ILoggingMessageQuee messageQueee, IStructuredLogMessageAppender messageAppender)
        {
            MessageQuee = messageQueee;
            MessageAppender = messageAppender;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new InMemoryLogger(MessageQuee,categoryName, MessageAppender);
        }
    }
}
