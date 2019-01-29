using Microsoft.Extensions.Logging;

namespace EnrichedExceptionLogging
{
    public class InMemoryLogginProvider : ILoggerProvider
    {
        public ILoggingMessageQuee MessageQuee { get; }

        public void Dispose()
        { }

        public InMemoryLogginProvider(ILoggingMessageQuee messageQueee)
        {
            MessageQuee = messageQueee;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new InMemoryLogger(MessageQuee,categoryName);
        }
    }
}
