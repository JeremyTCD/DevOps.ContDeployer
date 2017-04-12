using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace JeremyTCD.ContDeployer
{
    public class PluginFactory : IPluginFactory
    {
        private object NextPluginConfigOrOptions { get; set; }
        public IAssemblyService AssemblyService { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public Dictionary<string, Type> PluginOptionsTypes { get; set; }
        public Dictionary<string, Type> PluginTypes { get; set; }
        public ILogger<PluginFactory> Logger { get; set; }

        public PluginFactory(IServiceProvider serviceProvider,
            IAssemblyService assemblyService,
            ILogger<PluginFactory> logger)
        {
            ServiceProvider = serviceProvider;
            AssemblyService = assemblyService;
            Logger = logger;
        }

        public void LoadTypes()
        {
            // Get plugin assemblies 
            AssemblyService assemblyService = new AssemblyService();
            assemblyService.LoadAssembliesInDir(Path.Combine(Directory.GetCurrentDirectory(), "plugins"), true);
            IEnumerable<Assembly> pluginAssemblies = assemblyService.GetReferencingAssemblies(typeof(IPlugin).GetTypeInfo().Assembly);

            // Plugin options types
            PluginOptionsTypes = assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPluginOptions)).ToDictionary(type => type.Name);

            // Plugin types
            PluginTypes = assemblyService.GetAssignableTypes(pluginAssemblies, typeof(IPlugin)).ToDictionary(type => type.Name);
        }

        public IPlugin BuildPluginForPipelineStep(PipelineStep step)
        {
            PluginTypes.TryGetValue(step.PluginName, out Type pluginType);

            if(pluginType == null)
            {
                throw new Exception($"Plugin type with name: {step.PluginName} does not exist");
            }

            // If no value is provided for config in json, its value property will be an empty string
            if (step.Config != null && !string.IsNullOrEmpty(step.Config.Value))
            {
                NextPluginConfigOrOptions = step.Config;
            }
            else if(step.Options != null)
            {
                NextPluginConfigOrOptions = step.Options;
            }
            else
            {
                NextPluginConfigOrOptions = null;
                Logger.LogInformation($"No options provided for plugin: {step.PluginName}");
            }

            IPlugin plugin = ServiceProvider.GetService(pluginType) as IPlugin;

            if(plugin == null)
            {
                throw new Exception($"No service for type: {step.PluginName}");
            }

            return plugin;
        }

        public IPluginOptions BuildOptions(string name)
        {
            if (NextPluginConfigOrOptions is IPluginOptions)
            {
                return NextPluginConfigOrOptions as IPluginOptions;
            }

            IPluginOptions options = Activator.CreateInstance(PluginOptionsTypes[name]) as IPluginOptions;

            if (NextPluginConfigOrOptions is IConfigurationSection)
            {
                (NextPluginConfigOrOptions as IConfigurationSection).Bind(options);
            }

            return options;
        }
    }
}
