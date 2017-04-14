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
                AddSingleton<PipelineStepContextFactory>().
                AddSingleton<Pipeline>().
                Configure<PipelineOptions>(pipelineOptions =>
                {
                    ConfigurationBinder.Bind(_configurationRoot.GetSection("pipeline"), pipelineOptions);
                    pipelineOptions.Validate();
                });

            // Get plugin assemblies 
            AssemblyService assemblyService = new AssemblyService();
            assemblyService.LoadAssembliesInDir(Path.Combine(Directory.GetCurrentDirectory(), "plugins"), true);
            IEnumerable<Assembly> pluginAssemblies = assemblyService.GetReferencingAssemblies(typeof(IPlugin).GetTypeInfo().Assembly);

            // Plugin types
            IEnumerable<Type> pluginTypes = assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPlugin)).
                Concat(assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPluginOptions)));
            foreach (Type pluginType in pluginTypes)
            {
                services.AddTransient(pluginType);
            }

            // Plugin options types
            IEnumerable<Type> pluginOptionsTypes = assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPluginOptions));
            foreach (Type pluginOptionsType in pluginOptionsTypes)
            {
                services.AddTransient(pluginOptionsType, serviceProvider =>
                {
                    return serviceProvider.GetService<IPluginFactory>().BuildOptions(pluginOptionsType.Name);
                });
            }
        }

        public void Configure(ILoggerFactory loggerFactory, IPluginFactory pluginFactory)
        {
            loggerFactory.
                AddConsole(_configurationRoot.GetValue("Logging:LogLevel:Console", Microsoft.Extensions.Logging.LogLevel.Information)).
                AddDebug(_configurationRoot.GetValue("Logging:LogLevel:Debug", Microsoft.Extensions.Logging.LogLevel.Information));

            pluginFactory.LoadTypes();
        }
    }
}