using Microsoft.Extensions.Logging;
using StructureMap;

namespace JeremyTCD.ContDeployer
{
    public class ContDeployer
    {
        public static void Main(string[] args)
        {
            //CommandLineApplication app = new CommandLineApplication();
            //app.Name = "ContDeployer";
            //app.HelpOption("-h|--help");
            //app.OnExecute(() =>
            //{
            //    Console.WriteLine("Hello World!");
            //    return 0;
            //});

            //app.Execute(args);

            // All we need from args is log level
            // perhaps run / testrun

            Startup startup = new Startup();
            Container container = startup.ConfigureServices();

            // TODO How does asp net core call Configure with variable types as parameters?
            ILoggerFactory loggerFactory = container.GetInstance<ILoggerFactory>();
            IPluginFactory pluginFactory = container.GetInstance<IPluginFactory>();
            StepContextFactory stepContextFactory = container.GetInstance<StepContextFactory>();
            startup.Configure(loggerFactory, pluginFactory, stepContextFactory);

            Pipeline pipeline = container.GetInstance<Pipeline>();

            pipeline.Run();
        }
    }
}