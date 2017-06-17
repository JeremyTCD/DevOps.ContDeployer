using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using System.Linq;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class Startup
    {
        public IContainer ConfigureServices()
        {
            IServiceCollection pipelinesCEServices = new ServiceCollection();
            pipelinesCEServices.AddPipelinesCE();
            IContainer mainContainer = new Container(); // Use StructureMap for its multi-tenancy features
            mainContainer.Populate(pipelinesCEServices);

            // Use a StructureMap profile so that these CLA services are not exposed to plugins
            IServiceCollection claServices = new ServiceCollection();
            claServices.
                AddSingleton<RootCommand>().
                AddSingleton<RunCommand>().
                AddSingleton<ICommandLineUtilsService, CommandLineUtilsService>();
            mainContainer.Configure(configurationExpression =>
            {
                configurationExpression.Profile(nameof(CommandLineApp), registry =>
                {
                    ((Registry)registry).Populate(claServices);
                });
            });

            return mainContainer;
        }

        public void Configure(ILoggerFactory loggerFactory, string[] args)
        {
            bool verbose = args.Where(s => s == $"--{Strings.OptionLongName_Verbose}" ||
                s == $"-{Strings.OptionShortName_Verbose}").Count() > 0;

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
