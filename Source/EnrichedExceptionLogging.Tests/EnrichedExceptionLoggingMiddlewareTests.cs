using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace EnrichedExceptionLogging.Tests
{
    public class EnrichedExceptionLoggingMiddlewareTests
    {
        [Fact]
        public async void NextInvoked()
        {
            var rd = Substitute.For<RequestDelegate>();
            var eelm = new EnrichedExceptionLoggingMiddleware(rd, Substitute.For<ILoggerFactory>());
            var hhtpContext = new DefaultHttpContext();
            await eelm.InvokeAsync(hhtpContext,Substitute.For<ILoggingMessageQuee>());
            await rd.Received().Invoke(hhtpContext);
        }

        [Fact]
        public void ExceptionRethrown()
        {
            var rd = Substitute.For<RequestDelegate>();
            rd.Invoke(Arg.Any<HttpContext>()).Throws(new Exception("UnderlyingEx"));
            var eelm = new EnrichedExceptionLoggingMiddleware(rd, Substitute.For<ILoggerFactory>());

            Func<Task> f = async ()=> await eelm.InvokeAsync(new DefaultHttpContext(), Substitute.For<ILoggingMessageQuee>());
            f.Should().Throw<Exception>().WithMessage("UnderlyingEx");
        }

        [Fact]
        public async void LoggingQueeLogged()
        {
            var rd = Substitute.For<RequestDelegate>();
            rd.Invoke(Arg.Any<HttpContext>()).Throws(new Exception("UnderlyingEx"));
            var logger = Substitute.For<ILogger>();
            var lf = Substitute.For<ILoggerFactory>();
            lf.CreateLogger("").ReturnsForAnyArgs(logger);
            var eelm = new EnrichedExceptionLoggingMiddleware(rd, lf);

            var lmq = Substitute.For<ILoggingMessageQuee>();
            lmq.Count.Returns(2);
            var lm = new LoggingMessage
            {
                EventId = 1, 
                LogLevel = LogLevel.Trace,
                Message = "message"
            };
            lmq.Dequeue().Returns(lm);
            try
            {
                await eelm.InvokeAsync(new DefaultHttpContext(), lmq);
            }
            catch (Exception e)
            {
                // ignored
            }


            lmq.Received(2).Dequeue();
            //
            logger.Received().Log<object>(LogLevel.Error, lm.EventId,Arg.Is<FormattedLogValues>(flv=>flv.ToString() == "message"),
                null,Arg.Any<Func<object, Exception, string>>());
        }

        [Fact]
        public async void LoggingQueeCleared()
        {
            var rd = Substitute.For<RequestDelegate>();
            var eelm = new EnrichedExceptionLoggingMiddleware(rd, Substitute.For<ILoggerFactory>());
            var lmq = Substitute.For<ILoggingMessageQuee>();
            await eelm.InvokeAsync(new DefaultHttpContext(), lmq);
            lmq.Received().Clear();
        }
    }
}
