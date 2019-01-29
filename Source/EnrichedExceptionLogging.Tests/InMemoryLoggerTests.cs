using System;
using System.Collections.Generic;
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
            var iml = new InMemoryLogger(Substitute.For<ILoggingMessageQuee>(),"", Substitute.For<IStructuredLogMessageAppender>());
            iml.IsEnabled(LogLevel.Trace).Should().BeTrue();
            iml.IsEnabled(LogLevel.Error).Should().BeTrue();
        }

        [Fact]
        public void Log()
        {
            var lmq = Substitute.For<ILoggingMessageQuee>();
            var iml = new InMemoryLogger(lmq,"", Substitute.For<IStructuredLogMessageAppender>());
            var ex = new Exception();
            iml.Log(LogLevel.Trace, 11,"state", ex, Substitute.For<Func<object, Exception, string>>());
            lmq.Received().Enqueue(Arg.Is<LoggingMessage>(lm => lm.EventId == 11 && lm.LogLevel == LogLevel.Trace
                                                                && lm.State == "state" && lm.Exception == ex));
        }

        [Fact]
        public void Log_FormattedLogValues()
        {
            var lmq = Substitute.For<ILoggingMessageQuee>();
            var ma = Substitute.For<IStructuredLogMessageAppender>();
            var flv = new FormattedLogValues("","");
            ma.Append(null, LogLevel.None, "").ReturnsForAnyArgs(flv);

            var iml = new InMemoryLogger(lmq, "catName", ma);
            var lfm = new FormattedLogValues("{animal} eats {food}", "dog", "cat");

            var lm = new LoggingMessage();
            lmq.Enqueue(Arg.Do<LoggingMessage>(arg => lm = arg));
            iml.Log(LogLevel.Trace, 11, lfm, new Exception(), Substitute.For<Func<object, Exception, string>>());
            ma.Received().Append(lfm, LogLevel.Trace, "catName");

            lm.State.Should().BeEquivalentTo(flv);
        }

        [Fact]
        public void Log_KVPList()
        {
            var lmq = Substitute.For<ILoggingMessageQuee>();
            var ma = Substitute.For<IStructuredLogMessageAppender>();
            var flv = new FormattedLogValues("", "");
            ma.Append(null, LogLevel.None, "").ReturnsForAnyArgs(flv);
            var iml = new InMemoryLogger(lmq, "catName",ma);
            var state = new List<KeyValuePair<string, object>>(
                new[]
                {
                    new KeyValuePair<string, object>("{animal}", "dog"),
                    new KeyValuePair<string, object>("{food}", "cat"),
                    new KeyValuePair<string, object>("{OriginalMessage}", @"{animal} eats {food}")
                });
            var lm = new LoggingMessage();

            lmq.Enqueue(Arg.Do<LoggingMessage>(arg => lm = arg));

            iml.Log(LogLevel.Trace, 11, state, new Exception(), Substitute.For<Func<object, Exception, string>>());
            ma.Received().Append(state, LogLevel.Trace, "catName");
            lm.State.Should().BeEquivalentTo(flv);
        }
    }
}
