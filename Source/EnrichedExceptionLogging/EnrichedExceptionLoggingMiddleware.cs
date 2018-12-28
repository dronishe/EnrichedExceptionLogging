using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EnrichedExceptionLogging
{
    public class EnrichedExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public EnrichedExceptionLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {

            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = loggerFactory?.CreateLogger("EnrichedExceptionLogging") ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task InvokeAsync(HttpContext context, ILoggingMessageQuee messageQuee)
        {
            try
            {
                messageQuee.Clear();
                await _next(context);
            }
            catch (Exception)
            {
                var mq = messageQuee;
                for (var i = 0; i < messageQuee.Count; i++)
                {
                    var logEntry = mq.Dequeue();
                    _logger.LogError(logEntry.EventId, logEntry.Message);
                }
                  throw;
            }
        }
    }

    public static class EnrichedExceptionLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseEnrichedExceptionLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EnrichedExceptionLoggingMiddleware>();
        }
    }
}
