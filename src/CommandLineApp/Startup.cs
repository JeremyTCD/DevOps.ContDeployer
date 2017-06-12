using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.
                AddSingleton<RootCommand>().
                AddSingleton<RunCommand>().
                AddSingleton<ICommandLineUtilsService, CommandLineUtilsService>().
                AddSingleton(typeof(ILoggingService<>), typeof(LoggingService<>));

            services.
                AddOptions().
                AddLogging();

            // Separate container for PipelinesCE so that CLA services are hidden from plugins
            IServiceCollection pipelinesCEServices = new ServiceCollection();
            pipelinesCEServices.AddPipelinesCE();
            IContainer container = new Container(); // Use StructureMap for its multi-tenancy features
            container.Populate(pipelinesCEServices);
            services.
                AddSingleton(container);
        }

        public void Configure(ILoggerFactory loggerFactory, bool verbose)
        {
            // If need be, claOptions should be made configurable via json (using microsoft.extensions.configuration)
            CommandLineAppOptions claOptions = new CommandLineAppOptions();
            LogLevel logLevel = verbose ? claOptions.VerboseMinLogLevel : claOptions.DefaultMinLogLevel;

            loggerFactory.
                AddConsole(logLevel).
                AddFile(claOptions.LogFileFormat, logLevel).
                AddDebug(logLevel);
        }
    }
}
