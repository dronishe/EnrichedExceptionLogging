using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using NSubstitute;
using Xunit;

namespace EnrichedExceptionLogging.Tests
{
    public class InMemoryLoggerTests
    {
        [Fact]
        public void Enabled()
        {
            var iml = new InMemoryLogger(Substitute.For<ILoggingMessageQuee>(),"");
            iml.IsEnabled(LogLevel.Trace).Should().BeTrue();
            iml.IsEnabled(LogLevel.Error).Should().BeTrue();
        }

        [Fact]
        public void Log()
        {
            var lmq = Substitute.For<ILoggingMessageQuee>();
            var iml = new InMemoryLogger(lmq,"");
            var ex = new Exception();
            iml.Log(LogLevel.Trace, 11,"state", ex, Substitute.For<Func<object, Exception, string>>());
            lmq.Received().Enqueue(Arg.Is<LoggingMessage>(lm => lm.EventId == 11 && lm.LogLevel == LogLevel.Trace
                                                                && lm.State == "state" && lm.Exception == ex));
        }

        [Fact]
        public void Log_FormattedLogValues()
        {
            var lmq = Substitute.For<ILoggingMessageQuee>();
            var iml = new InMemoryLogger(lmq, "catName");
            var ex = new Exception();
            var lfm = new FormattedLogValues("{animal} eats {food}", "dog", "cat");
            var lm = new LoggingMessage();
            lmq.Enqueue(Arg.Do<LoggingMessage>(arg => lm = arg));
            iml.Log(LogLevel.Trace, 11, lfm, ex, Substitute.For<Func<object, Exception, string>>());
            var flv = lm.State as FormattedLogValues;
            var message = flv.Last().Value as string;
            message.Should().ContainAll(new[] {"TimeStamp", "OriginalLogLevel", "CategoryName"});
            flv.Select(kvp => kvp.Value.ToString()).Should().Contain(new[] {"Trace", "catName"});
        }

    }
}
