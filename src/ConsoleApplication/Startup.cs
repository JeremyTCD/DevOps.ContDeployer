using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StructureMap;
using System.IO;

namespace JeremyTCD.ContDeployer.ConsoleApplication
{
    public class Startup
    {
        private IConfigurationRoot _configurationRoot { get; }

        public Startup()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().
                SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile("cd.json", false);
            _configurationRoot = builder.Build();
        }

        public void ConfigureServices(IContainer main)
        {
            main.AddContDeployer(_configurationRoot);
        }

        public void Configure(ILoggerFactory loggerFactory, IPluginFactory pluginFactory,
            IStepContextFactory stepContextFactory)
        {
            loggerFactory.
                AddFile(_configurationRoot.GetValue("Logging:File:LogFile", "log.txt"),
                    (_configurationRoot.GetValue("Logging:File:LogLevel", LogLevel.Information))).
                AddConsole(_configurationRoot.GetValue("Logging:Console:LogLevel", LogLevel.Information)).
                AddDebug(_configurationRoot.GetValue("Logging:Debug:LogLevel", LogLevel.Information));
            pluginFactory.LoadTypes();
            stepContextFactory.LoadTypes();
        }
    }
}
