using System;
using System.Linq;
using EnrichedExceptionLogging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;

namespace EnrichedExceptionLoggingExample
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Log.Logger
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .WriteTo.RollingFile(new JsonFormatter(null,true), "Logs\\Json-log-{Date}.txt")
                .WriteTo.RollingFile("Logs\\log-{Date}.txt")
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(lb => lb.AddSerilog(dispose: true));

            services.AddMvc();
            
            services.AddEnrichedExceptionLogging();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider sp)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseEnrichedExceptionLoggingMiddleware(false);
            app.UseMvc();
        }
    }
}
