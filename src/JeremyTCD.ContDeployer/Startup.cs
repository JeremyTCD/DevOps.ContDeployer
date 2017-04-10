using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
            services.
                AddLogging().
                AddOptions();

            services.
                AddSingleton<IAssemblyService, AssemblyService>().
                AddSingleton<IRepository>(provider => new Repository(Directory.GetCurrentDirectory()));

            services.
                AddSingleton<Pipeline>();
            services.Configure<PipelineOptions>(pipelineOptions =>
            {
                ConfigurationBinder.Bind(_configurationRoot.GetSection("Pipeline"), pipelineOptions);
                pipelineOptions.Validate();
            });

            // Get plugin assemblies 
            AssemblyService assemblyService = new AssemblyService();
            assemblyService.LoadAssembliesInDir(Path.Combine(Directory.GetCurrentDirectory(), "plugins"), true);
            IEnumerable<Assembly> pluginAssemblies = assemblyService.GetReferencingAssemblies(typeof(IPlugin).GetTypeInfo().Assembly);

            // Get plugin and plugin options types
            IEnumerable<Type> types = assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPlugin)).
                Concat(assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPluginOptions)));

            foreach (Type type in types)
            {
                services.AddTransient(type);
            }
        }

        public void Configure(ILoggerFactory loggerFactory)
        {
            loggerFactory.
                AddConsole(_configurationRoot.GetValue("Logging:LogLevel:Console", Microsoft.Extensions.Logging.LogLevel.Information)).
                AddDebug(_configurationRoot.GetValue("Logging:LogLevel:Debug", Microsoft.Extensions.Logging.LogLevel.Information));
        }
    }
}