using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Xunit;

namespace EnrichedExceptionLogging.Tests
{
    public class StructuredLogMessageAppenderTests
    {
        [Fact]
        public void Append()
        {
            var origin = new List<KeyValuePair<string, object>>(
                new[]
                {
                    new KeyValuePair<string, object>("{animal}", "dog"),
                    new KeyValuePair<string, object>("{food}", "cat"),
                    new KeyValuePair<string, object>("{OriginalMessage}", @"{animal} eats {food}")
                });
            var res = new StructuredLogMessageAppender().Append(origin, LogLevel.Trace, "catName");
            res.Should().BeOfType<FormattedLogValues>();
            var message = res.Last().Value as string;
            message.Should().ContainAll(new[] { "OriginalTimeStamp", "OriginalLogLevel", "OriginalSourceContext", "animal", "food" });
            res.Select(kvp => kvp.Value.ToString()).Should().Contain(new[] { "Trace", "catName" });
        }
    }
}
