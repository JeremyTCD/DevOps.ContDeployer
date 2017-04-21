using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Octokit;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace JeremyTCD.ContDeployer
{
    public class Startup
    {
        private IConfigurationRoot _configurationRoot { get; set; }
        private IAssemblyService _assemblyService { get; set; }

        public Startup()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().
                SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile("cd.json", false);
            _configurationRoot = builder.Build();

            _assemblyService = new AssemblyService();
        }

        public Container ConfigureServices()
        {
            Container main = new Container();
            ServiceCollection services = new ServiceCollection();
            // Services for external types
            services.
                AddLogging().
                AddOptions().
                AddSingleton<IAssemblyService, AssemblyService>().
                AddSingleton<IRepository>(provider => new LibGit2Sharp.Repository(Directory.GetCurrentDirectory())).
                AddSingleton<HttpClient>().
                AddSingleton<IContainer>(main);

            services.
                AddSingleton<IProcessManager, ProcessManager>().
                AddSingleton<IHttpManager, HttpManager>().
                AddSingleton<IPluginFactory, PluginFactory>().
                AddSingleton<Pipeline>().
                AddSingleton<PipelineContext>().
                Configure<PipelineContextOptions>(options =>
                {
                    ConfigurationBinder.Bind(_configurationRoot.GetSection("pipeline"), options);
                    options.Validate();
                }).
                AddSingleton<StepContextFactory>().
                AddTransient(provider => provider.GetService<StepContextFactory>().Build()).
                Configure<SharedOptions>(_configurationRoot.GetSection("shared"));
            ConfigurePluginServices(services);
            main.Populate(services);

            // Load assemblies in plugins directory
            _assemblyService.LoadAssembliesInDir(Path.Combine(Directory.GetCurrentDirectory(), "plugins"), true);
            ConfigurePluginContainers(main);

            return main;
        }

        /// <summary>
        /// Adds services for plugins. Adds to main service collection since some plugins may not have their own containers.
        /// </summary>
        /// <param name="services"></param>
        private void ConfigurePluginServices(ServiceCollection services)
        {
            // Plugins
            IEnumerable<Assembly> pluginAssemblies = _assemblyService.GetReferencingAssemblies(typeof(IPlugin).GetTypeInfo().Assembly);
            List<Type> pluginTypes = _assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPlugin)).ToList();

            foreach (Type pluginType in pluginTypes)
            {
                services.AddTransient(pluginType);
            }
        }

        private void ConfigurePluginContainers(Container main)
        {
            IEnumerable<Assembly> pluginAssemblies = _assemblyService.GetReferencingAssemblies(typeof(IPlugin).GetTypeInfo().Assembly);
            List<Type> pluginStartupTypes = _assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPluginStartup)).ToList();
            Dictionary<string, IContainer> children = new Dictionary<string, IContainer>();

            foreach (Type pluginStartupType in pluginStartupTypes)
            {
                IPluginStartup pluginStartup = (IPluginStartup)Activator.CreateInstance(pluginStartupType);
                ServiceCollection pluginServices = new ServiceCollection();
                pluginStartup.ConfigureServices(pluginServices);

                IContainer child = main.CreateChildContainer();
                child.Configure(config => config.Populate(pluginServices));
                children.Add(pluginStartupType.Name.Replace("Startup", ""), child);
            }

            main.Configure(config => config.ForSingletonOf<IDictionary<string, IContainer>>().Use(children));
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