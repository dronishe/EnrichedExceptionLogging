using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EnrichedExceptionLogging
{
    public class EnrichedExceptionLoggingMiddleware
    {
        public ILoggerFactory LoggerFactory { get; }
        public bool RethrowException { get; }
        private readonly RequestDelegate _next;

        public EnrichedExceptionLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, bool rethrowException)
        {
            LoggerFactory = loggerFactory;
            RethrowException = rethrowException;

            _next = next ?? throw new ArgumentNullException(nameof(next));
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
                var pp = messageQuee.Count;
                var messages = new List<LoggingMessage>();

                var logger = LoggerFactory?.CreateLogger("EnrichedExceptionLogging") ?? throw new ArgumentNullException(nameof(LoggerFactory));
                for (var i = 0; i < pp; i++)
                    messages.Add(messageQuee.Dequeue());

                foreach (var logEntry in messages)

                    logger.Log<object>(LogLevel.Error, logEntry.EventId, logEntry.State, logEntry.Exception,
                        ((o, exception) => o.ToString()));

                if(RethrowException)
                  throw;
            }
        }
    }

    public static class EnrichedExceptionLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseEnrichedExceptionLoggingMiddleware(this IApplicationBuilder builder, bool rethrowException = true)
        {
            return builder.UseMiddleware<EnrichedExceptionLoggingMiddleware>(rethrowException);
        }
    }
}
