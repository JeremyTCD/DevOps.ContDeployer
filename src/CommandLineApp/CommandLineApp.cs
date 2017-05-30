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

            Startup startup = new Startup();
            IServiceCollection services = new ServiceCollection();
            startup.ConfigureServices(services);

            // Wrap services in a StructureMap container to utilize its multi-tenancy features
            IContainer mainContainer = new Container();
            mainContainer.Populate(services);

            ILogger<CommandLineApp> logger = mainContainer.GetInstance<ILogger<CommandLineApp>>();
            RootCommand rootCommand = mainContainer.GetInstance<RootCommand>();
            args = args.Select(s => s.ToLowerInvariant()).ToArray();

            try
            {
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
