using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace JeremyTCD.DevOps.ContDeployer
{
    class ContDeployer
    {
        static void Main(string[] args)
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

            // Startup
            // Create pipeline
            // Start pipeline

            Startup startup = new Startup();

            ServiceCollection services = new ServiceCollection();
            startup.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            startup.Configure(loggerFactory);

            ILogger pipelineLogger = serviceProvider.GetService<ILogger<Pipeline>>();


        }
    }
}