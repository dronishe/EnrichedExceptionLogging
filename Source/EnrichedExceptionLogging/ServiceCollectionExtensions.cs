using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EnrichedExceptionLogging
{
    public static class EnrichedExceptionLoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddEnrichedExceptionLogging(this IServiceCollection services)
        {

            services.AddSingleton<InMemoryLogginProvider>();
            services.AddSingleton<ILoggingMessageQuee, LoggingMessageQuee>();

            services.AddLogging(
                builder =>
                {
                    builder.Services.AddSingleton<ILoggerProvider, InMemoryLogginProvider>();
                    builder.AddFilter<InMemoryLogginProvider>(level => true);

                }
            );
            return services;
        }
    }
}
