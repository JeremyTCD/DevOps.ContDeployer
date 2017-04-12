using JeremyTCD.ContDeployer.PluginTools;
using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Configuration;
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


        public PluginFactory(IServiceProvider serviceProvider,
            IAssemblyService assemblyService)
        {
            ServiceProvider = serviceProvider;
            AssemblyService = assemblyService;
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

        public IPlugin BuildPlugin(string name, object options)
        {
            PluginTypes.TryGetValue(name, out Type pluginType);

            if(pluginType == null)
            {
                throw new Exception($"Plugin type with name: {name} does not exist");
            }

            NextPluginConfigOrOptions = options;

            IPlugin plugin = ServiceProvider.GetService(pluginType) as IPlugin;

            if(plugin == null)
            {
                throw new Exception($"No service for type: {name}");
            }

            return plugin;
        }

        public IPluginOptions BuildOptions(string name)
        {
            if (NextPluginConfigOrOptions is IPluginOptions)
            {
                return NextPluginConfigOrOptions as IPluginOptions;
            }
            else if(NextPluginConfigOrOptions is IConfigurationSection)
            {
                IPluginOptions options = Activator.CreateInstance(PluginOptionsTypes[name]) as IPluginOptions;
                (NextPluginConfigOrOptions as IConfigurationSection).Bind(options);

                return options;
            }

            return null;
        }
    }
}
