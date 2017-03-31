using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JeremyTCD.DevOps.ContDeployer
{
    class Startup
    {
        public static IConfigurationRoot _configurationRoot { get; set; }

        public Startup()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().
                SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile("cd.json", false);
            _configurationRoot = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.
                AddLogging().
                AddOptions();

            services.Configure<PipelineFactoryOptions>(_configurationRoot.GetSection("Pipeline"));
        }

        public void Configure(ILoggerFactory loggerFactory)
        {
            loggerFactory.
                AddConsole((LogLevel)Enum.Parse(typeof(LogLevel), _configurationRoot["Logging:LogLevel:Console"])).
                AddDebug((LogLevel)Enum.Parse(typeof(LogLevel), _configurationRoot["Logging:LogLevel:Debug"]));
        }
    }
}