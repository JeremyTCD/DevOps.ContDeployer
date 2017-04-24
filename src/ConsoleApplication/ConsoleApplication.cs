using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using System;

namespace JeremyTCD.ContDeployer.ConsoleApplication
{
    public class ConsoleApplication
    {
        public static void Main(string[] args)
        {
            Startup startup = new Startup();
            IContainer main = new Container();
            startup.ConfigureServices(main);

            // TODO How does asp net core call Configure with variable types as parameters?
            ILoggerFactory loggerFactory = main.GetInstance<ILoggerFactory>();
            IPluginFactory pluginFactory = main.GetInstance<IPluginFactory>();
            IStepContextFactory stepContextFactory = main.GetInstance<IStepContextFactory>();
            startup.Configure(loggerFactory, pluginFactory, stepContextFactory);

            IPipeline pipeline = main.GetInstance<IPipeline>();

            pipeline.Run();
        }
    }
}
