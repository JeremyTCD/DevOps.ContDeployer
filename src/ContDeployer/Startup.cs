using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace JeremyTCD.ContDeployer
{
    class Startup
    {
        public static IConfigurationRoot _configurationRoot { get; set; }

        public Startup()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().
                SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile("cd.json", false);
            _configurationRoot = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Services for external types
            services.
                AddLogging().
                AddOptions().
                AddSingleton<IAssemblyService, AssemblyService>().
                AddSingleton<IRepository>(provider => new Repository(Directory.GetCurrentDirectory()));

            services.
                AddSingleton<IProcessManager, ProcessManager>().
                AddSingleton<IPluginFactory, PluginFactory>().
                AddSingleton<PipelineContextFactory>().
                AddSingleton<StepContextFactory>().
                AddSingleton<Pipeline>().
                Configure<PipelineOptions>(pipelineOptions =>
                {
                    ConfigurationBinder.Bind(_configurationRoot.GetSection("pipeline"), pipelineOptions);
                    pipelineOptions.Validate();
                }).
                Configure<SharedOptions>(_configurationRoot.GetSection("shared"));
        }

        public void Configure(ILoggerFactory loggerFactory, IPluginFactory pluginFactory, 
            StepContextFactory stepContextFactory)
        {
            loggerFactory.
                AddConsole(_configurationRoot.GetValue("Logging:LogLevel:Console", Microsoft.Extensions.Logging.LogLevel.Information)).
                AddDebug(_configurationRoot.GetValue("Logging:LogLevel:Debug", Microsoft.Extensions.Logging.LogLevel.Information));

            pluginFactory.LoadTypes();
            stepContextFactory.LoadTypes();
        }
    }
}