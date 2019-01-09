using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace EnrichedExceptionLogging.Tests
{
    public class InMemoryLoggerTests
    {
        [Fact]
        public void Enabled()
        {
            var iml = new InMemoryLogger(Substitute.For<ILoggingMessageQuee>());
            iml.IsEnabled(LogLevel.Trace).Should().BeTrue();
            iml.IsEnabled(LogLevel.Error).Should().BeTrue();
        }

        [Fact]
        public void Log()
        {
            var lmq = Substitute.For<ILoggingMessageQuee>();
            var formatter = Substitute.For<Func<object, Exception, string>>();
            formatter.Invoke(null, null).ReturnsForAnyArgs("formatted");
            var iml = new InMemoryLogger(lmq);
            var ex = new Exception();
            iml.Log(LogLevel.Trace, 11,"state", ex, formatter);
            //lmq.Received().Enqueue(Arg.Is<LoggingMessage>(lm => lm.EventId == 11 && lm.Message=="formatted"));
            lmq.Received().Enqueue(
            new LoggingMessage
            {
                EventId = 11,
                LogLevel = LogLevel.Trace,
                Message = "formatted"
            });
            formatter.Received().Invoke("state", ex);
        }
    }
}
