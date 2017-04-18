using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net.Http;

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
                AddSingleton<IRepository>(provider => new Repository(Directory.GetCurrentDirectory())).
                AddSingleton<HttpClient>();

            services.
                AddSingleton<IProcessManager, ProcessManager>().
                AddSingleton<IHttpManager, HttpManager>().
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
                AddFile(_configurationRoot.GetValue("Logging:File:LogFile", "log.txt"),
                    (_configurationRoot.GetValue("Logging:File:LogLevel", Microsoft.Extensions.Logging.LogLevel.Information))).
                AddConsole(_configurationRoot.GetValue("Logging:Console:LogLevel", Microsoft.Extensions.Logging.LogLevel.Information)).
                AddDebug(_configurationRoot.GetValue("Logging:Debug:LogLevel", Microsoft.Extensions.Logging.LogLevel.Information));

            pluginFactory.LoadTypes();
            stepContextFactory.LoadTypes();
        }
    }
}