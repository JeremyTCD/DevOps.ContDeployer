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
        public static IConfigurationRoot _configurationRoot { get; set; }

        public Startup()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().
                SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile("cd.json", false);
            _configurationRoot = builder.Build();
        }

        public Container ConfigureServices()
        {
            ServiceCollection services = new ServiceCollection();

            // Services for external types
            services.
                AddLogging().
                AddOptions().
                AddSingleton<IAssemblyService, AssemblyService>().
                AddSingleton<IRepository>(provider => new LibGit2Sharp.Repository(Directory.GetCurrentDirectory())).
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

            Container main = new Container();
            main.Populate(services);

            ConfigureChildContainers(main);

            return main;
        }

        private void ConfigurePluginServices(ServiceCollection services)
        {
            // Add services for plugins
        }

        private void ConfigureChildContainers(Container main)
        {
            IAssemblyService assemblyService = main.GetInstance<IAssemblyService>();
            // Load assemblies in plugins directory
            assemblyService.LoadAssembliesInDir(Path.Combine(Directory.GetCurrentDirectory(), "plugins"), true);
            IEnumerable<Assembly> pluginAssemblies = assemblyService.GetReferencingAssemblies(typeof(IPlugin).GetTypeInfo().Assembly);
            List<Type> pluginStartupTypes = assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPluginStartup)).ToList();
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