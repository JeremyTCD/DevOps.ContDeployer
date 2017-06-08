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
                AddSingleton<ICommandLineUtilsService, CommandLineUtilsService>();

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

        public void Configure(ILoggerFactory loggerFactory)
        {
            // If need be, claOptions can be made configurable via microsoft.extensions.configuration 
            CommandLineAppOptions claOptions = new CommandLineAppOptions();

            loggerFactory.
                AddConsole(claOptions.DefaultMinLogLevel).
                AddFile(claOptions.LogFileFormat, claOptions.DefaultMinLogLevel).
                AddDebug(claOptions.DefaultMinLogLevel);
        }
    }
}
