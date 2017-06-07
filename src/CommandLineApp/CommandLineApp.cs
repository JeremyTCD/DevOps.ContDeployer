using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using System;
using System.Linq;

namespace JeremyTCD.PipelinesCE.CommandLineApp
{
    public class CommandLineApp
    {
        public static int Main(string[] args)
        {
            // Instantiate catchall logger
            LoggerFactory loggerFactory = new LoggerFactory();
            CommandLineAppOptions claOptions = new CommandLineAppOptions();
            loggerFactory.
                AddConsole(claOptions.DefaultMinLogLevel).
                AddFile(claOptions.LogFileFormat, claOptions.DefaultMinLogLevel).
                AddDebug(claOptions.DefaultMinLogLevel);
            ILogger<CommandLineApp> logger = loggerFactory.CreateLogger<CommandLineApp>();

            try
            {
                Startup startup = new Startup();
                IServiceCollection services = new ServiceCollection();
                startup.ConfigureServices(services);

                // Wrap services in a StructureMap container to utilize its multi-tenancy features
                IContainer mainContainer = new Container();
                mainContainer.Populate(services);

                RootCommand rootCommand = mainContainer.GetInstance<RootCommand>();
                args = args.Select(s => s.ToLowerInvariant()).ToArray();

                rootCommand.Execute(args);
            }
            catch (Exception e)
            {
                // Catch unhandled exceptions and log them using logger. This ensures that unhandled exceptions are logged by all
                // logging providers (such as file, debug etc - not just console).
                logger.LogError(e.ToString());
                return 1;
            }

            return 0;
        }
    }
}
