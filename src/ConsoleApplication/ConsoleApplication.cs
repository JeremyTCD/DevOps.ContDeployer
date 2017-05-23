using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using System;

namespace JeremyTCD.PipelinesCE.ConsoleApplication
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
            startup.Configure(loggerFactory);

            IPipeline pipeline = main.GetInstance<IPipeline>();

            pipeline.Run();
        }
    }
}
