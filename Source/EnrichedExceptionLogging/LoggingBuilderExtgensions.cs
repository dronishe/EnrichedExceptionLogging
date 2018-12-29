using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EnrichedExceptionLogging
{
    public static class EnrichedExceptionLoggingBuilderExtensions
    {
        public static ILoggingBuilder AddProvider<T>(this ILoggingBuilder builder, Func<IServiceProvider, T> providerCreator)
            where T : class, ILoggerProvider
        {
            builder.Services.AddSingleton<ILoggerProvider, T>(providerCreator);
            return builder;
        }
    }
}
